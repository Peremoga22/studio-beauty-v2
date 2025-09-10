(function () {
    "use strict";

    function throttle(fn) {
        window.setTimeout(fn, 1000 / 60); // 60 FPS
    }

    let contextCounter = 0;
    const contexts = {};

    function Context(element) {
        this.element = element;
        this.Adapter = Waypoint.Adapter;
        this.adapter = new this.Adapter(element);

        this.key = "waypoint-context-" + contextCounter;
        contextCounter += 1;

        // Стани
        this.didScroll = false;
        this.didResize = false;

        this.oldScroll = {
            x: this.adapter.scrollLeft(),
            y: this.adapter.scrollTop()
        };

        // Вертикальні та горизонтальні waypoints
        this.waypoints = {
            vertical: {},
            horizontal: {}
        };

        // Прив'язуємо ключ
        element.waypointContextKey = this.key;
        contexts[this.key] = this;

        // Для window контексту — лише один
        if (!Waypoint.windowContext && element === window) {
            Waypoint.windowContext = this;
        }

        // Обробники прокрутки та resize
        this.createScrollHandler();
        this.createResizeHandler();
    }

    // Додати waypoint до контексту
    Context.prototype.add = function (waypoint) {
        const axis = waypoint.options.horizontal ? "horizontal" : "vertical";
        this.waypoints[axis][waypoint.key] = waypoint;
        this.refresh();
    };

    // Видалити waypoint
    Context.prototype.remove = function (waypoint) {
        delete this.waypoints[waypoint.axis][waypoint.key];
        this.checkEmpty();
    };

    // Перевірити, чи немає waypoint'ів
    Context.prototype.checkEmpty = function () {
        const noHorizontals = this.Adapter.isEmptyObject(this.waypoints.horizontal);
        const noVerticals = this.Adapter.isEmptyObject(this.waypoints.vertical);
        const isWindow = this.element === window;

        if (noHorizontals && noVerticals && !isWindow) {
            this.adapter.off(".waypoints");
            delete contexts[this.key];
        }
    };

    // Scroll handler
    Context.prototype.createScrollHandler = function () {
        const self = this;

        function onScroll() {
            self.handleScroll();
            self.didScroll = false;
        }

        this.adapter.on("scroll.waypoints", function () {
            if (!self.didScroll || Waypoint.isTouch) {
                self.didScroll = true;
                Waypoint.requestAnimationFrame(onScroll);
            }
        });
    };

    // Resize handler
    Context.prototype.createResizeHandler = function () {
        const self = this;

        function onResize() {
            Waypoint.Context.refreshAll();
            self.didResize = false;
        }

        this.adapter.on("resize.waypoints", function () {
            if (!self.didResize) {
                self.didResize = true;
                Waypoint.requestAnimationFrame(onResize);
            }
        });
    };

    // Обробка скролу
    Context.prototype.handleScroll = function () {
        const axes = {
            horizontal: {
                newScroll: this.adapter.scrollLeft(),
                oldScroll: this.oldScroll.x,
                forward: "right",
                backward: "left"
            },
            vertical: {
                newScroll: this.adapter.scrollTop(),
                oldScroll: this.oldScroll.y,
                forward: "down",
                backward: "up"
            }
        };

        const triggeredGroups = {};

        for (let axis in axes) {
            const { newScroll, oldScroll, forward, backward } = axes[axis];
            const goingForward = newScroll > oldScroll;

            for (let key in this.waypoints[axis]) {
                const wp = this.waypoints[axis][key];

                if (wp.triggerPoint == null) continue;

                const crossedForward = oldScroll < wp.triggerPoint && newScroll >= wp.triggerPoint;
                const crossedBackward = oldScroll > wp.triggerPoint && newScroll <= wp.triggerPoint;

                if (crossedForward || crossedBackward) {
                    wp.queueTrigger(goingForward ? forward : backward);
                    triggeredGroups[wp.group.id] = wp.group;
                }
            }
        }

        for (let groupId in triggeredGroups) {
            triggeredGroups[groupId].flushTriggers();
        }

        this.oldScroll = {
            x: axes.horizontal.newScroll,
            y: axes.vertical.newScroll
        };
    };

    // Вимірювання
    Context.prototype.innerHeight = function () {
        return this.element === window
            ? Waypoint.viewportHeight()
            : this.adapter.innerHeight();
    };

    Context.prototype.innerWidth = function () {
        return this.element === window
            ? Waypoint.viewportWidth()
            : this.adapter.innerWidth();
    };

    // Оновлення позицій
    Context.prototype.refresh = function () {
        const isWindow = this.element === window;
        const offset = isWindow ? undefined : this.adapter.offset();
        const triggeredGroups = {};

        const axes = {
            horizontal: {
                contextOffset: isWindow ? 0 : offset.left,
                contextScroll: isWindow ? 0 : this.oldScroll.x,
                contextDimension: this.innerWidth(),
                oldScroll: this.oldScroll.x,
                forward: "right",
                backward: "left",
                offsetProp: "left"
            },
            vertical: {
                contextOffset: isWindow ? 0 : offset.top,
                contextScroll: isWindow ? 0 : this.oldScroll.y,
                contextDimension: this.innerHeight(),
                oldScroll: this.oldScroll.y,
                forward: "down",
                backward: "up",
                offsetProp: "top"
            }
        };

        for (let axis in axes) {
            const { contextOffset, contextScroll, contextDimension, oldScroll, forward, backward, offsetProp } = axes[axis];

            for (let key in this.waypoints[axis]) {
                const wp = this.waypoints[axis][key];
                let offsetVal = wp.options.offset;
                let elementOffset = 0;

                if (wp.element !== wp.element.window) {
                    elementOffset = wp.adapter.offset()[offsetProp];
                }

                if (typeof offsetVal === "function") {
                    offsetVal = offsetVal.apply(wp);
                } else if (typeof offsetVal === "string") {
                    offsetVal = parseFloat(offsetVal);
                    if (wp.options.offset.indexOf("%") > -1) {
                        offsetVal = Math.ceil(contextDimension * offsetVal / 100);
                    }
                }

                const triggerPoint = Math.floor(elementOffset + contextScroll - contextOffset - offsetVal);
                const wasBefore = wp.triggerPoint < oldScroll;
                const nowAfter = triggerPoint >= oldScroll;

                wp.triggerPoint = triggerPoint;

                const crossedBackward = wasBefore && nowAfter;
                const crossedForward = !wasBefore && !nowAfter;

                if (wp.triggerPoint != null && crossedBackward) {
                    wp.queueTrigger(backward);
                    triggeredGroups[wp.group.id] = wp.group;
                } else if (wp.triggerPoint != null && crossedForward) {
                    wp.queueTrigger(forward);
                    triggeredGroups[wp.group.id] = wp.group;
                }
            }
        }

        Waypoint.requestAnimationFrame(() => {
            for (let id in triggeredGroups) {
                triggeredGroups[id].flushTriggers();
            }
        });

        return this;
    };

    Context.findOrCreateByElement = function (element) {
        return Context.findByElement(element) || new Context(element);
    };

    Context.findByElement = function (element) {
        return contexts[element.waypointContextKey];
    };

    Context.refreshAll = function () {
        for (let key in contexts) {
            contexts[key].refresh();
        }
    };

    // requestAnimationFrame fallback
    Waypoint.requestAnimationFrame = function (callback) {
        const raf =
            window.requestAnimationFrame ||
            window.mozRequestAnimationFrame ||
            window.webkitRequestAnimationFrame ||
            throttle;
        raf.call(window, callback);
    };

    Waypoint.Context = Context;
})();
