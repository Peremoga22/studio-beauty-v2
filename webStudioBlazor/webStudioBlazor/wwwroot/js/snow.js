// Snow Effect JavaScript
(function() {
    'use strict';

    // Символи сніжинок
    /** 
     * Array of snowflake characters used to render snowflakes.
     * Chosen as string[] because each entry is a single-character string.
     * @type {string[]}
     */
    const snowflakes = ['❄', '❅', '❆', '✻', '✼', '✽', '✾', '✿', '❀', '❁'];
    
    // Кількість сніжинок
    /**
     * Number of snowflakes to create initially.
     * @type {number}
     */
    const snowflakeCount = 50;
    
    // Створення контейнера для снігу
    /**
     * Create and append the main snow container div.
     * Returns the created HTMLDivElement so callers can use DOM APIs safely.
     * @returns {HTMLDivElement}
     */
    function createSnowContainer() {
        /** @type {HTMLDivElement} */
        const container = document.createElement('div');
        container.className = 'snow-container';
        container.id = 'snow-container';
        document.body.appendChild(container);
        return container;
    }
    
    // Створення однієї сніжинки
    /**
     * Create a single snowflake element and append it to the provided container.
     * The container is expected to be a div element created by createSnowContainer.
     * @param {HTMLDivElement} container - The container to append the snowflake to.
     * @returns {void}
     */
    function createSnowflake(container) {
        /** @type {HTMLDivElement} */
        const snowflake = document.createElement('div');
        snowflake.className = 'snowflake';
        
        // Випадковий символ сніжинки
        /** @type {string} */
        const randomSymbol = snowflakes[Math.floor(Math.random() * snowflakes.length)];
        snowflake.textContent = randomSymbol;
        
        // Випадковий розмір
        /** @type {number} */
        const size = Math.random();
        if (size < 0.3) {
            snowflake.classList.add('small');
        } else if (size < 0.7) {
            snowflake.classList.add('medium');
        } else {
            snowflake.classList.add('large');
        }
        
        // Випадкова позиція по горизонталі
        // left value represented as a percentage string
        snowflake.style.left = Math.random() * 100 + '%';
        
        // Випадкова швидкість падіння
        /** @type {number} */
        const duration = 8 + Math.random() * 7; // від 8 до 15 секунд
        snowflake.style.animationDuration = duration + 's';
        
        // Випадкова затримка
        snowflake.style.animationDelay = Math.random() * 3 + 's';
        
        // Додавання коливання для деяких сніжинок
        if (Math.random() > 0.5) {
            snowflake.classList.add('sway');
        }
        
        // Випадковий початковий розмір
        /** @type {number} */
        const scale = 0.5 + Math.random() * 0.5;
        snowflake.style.transform = `scale(${scale})`;
        
        container.appendChild(snowflake);
        
        // Видалення сніжинки після падіння
        // duration is in seconds, so convert to milliseconds for setTimeout
        setTimeout(() => {
            if (snowflake.parentNode) {
                snowflake.remove();
                // Створення нової сніжинки
                createSnowflake(container);
            }
        }, duration * 1000);
    }
    
    // Змінна для відстеження чи вже ініціалізовано
    let isInitialized = false;
    let snowflakeInterval = null;

    // Ініціалізація ефекту снігу
    /**
     * Initialize the snow effect: ensure container exists and spawn initial snowflakes.
     * Uses getElementById which may return null, so handle that case.
     * @returns {void}
     */
    function initSnow() {
        // Перевірка, чи вже існує контейнер
        /** @type {HTMLElement | null} */
        let container = document.getElementById('snow-container');
        if (!container) {
            container = createSnowContainer();
        }
        
        // Якщо вже ініціалізовано, не створюємо нові сніжинки
        if (isInitialized) {
            return;
        }
        
        isInitialized = true;
        
        // Створення початкових сніжинок
        for (let i = 0; i < snowflakeCount; i++) {
            setTimeout(() => {
                // Перевіряємо, чи контейнер все ще існує
                const currentContainer = document.getElementById('snow-container');
                if (currentContainer) {
                    createSnowflake(/** @type {HTMLDivElement} */(currentContainer));
                }
            }, i * 200); // Поступове створення
        }
    }
    
    // Функція для перевірки та відновлення снігу
    function ensureSnowRunning() {
        const container = document.getElementById('snow-container');
        if (!container) {
            // Якщо контейнер зник (наприклад, при навігації), створюємо знову
            isInitialized = false;
            initSnow();
        } else if (!isInitialized) {
            // Якщо контейнер є, але не ініціалізовано
            initSnow();
        }
    }
    
    // Запуск після завантаження сторінки
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSnow);
    } else {
        initSnow();
    }
    
    // Експорт функції для можливості керування з Blazor
    /**
     * Expose a simple API for controlling the snow effect from other scripts (e.g., Blazor).
     * @type {{init: function():void, stop: function():void, start: function():void, ensure: function():void}}
     */
    window.snowEffect = {
        init: initSnow,
        stop: function() {
            /** @type {HTMLElement | null} */
            const container = document.getElementById('snow-container');
            if (container) {
                container.remove();
                isInitialized = false;
            }
        },
        start: function() {
            isInitialized = false;
            initSnow();
        },
        ensure: ensureSnowRunning
    };
    
    // Перевірка наявності контейнера при зміні DOM (для Blazor навігації)
    // Використовуємо debounce для уникнення занадто частих викликів
    let ensureTimeout = null;
    function debouncedEnsure() {
        if (ensureTimeout) {
            clearTimeout(ensureTimeout);
        }
        ensureTimeout = setTimeout(function() {
            ensureSnowRunning();
        }, 500); // Перевіряємо через 500мс після останньої зміни
    }
    
    // Перевірка наявності контейнера при зміні DOM (для Blazor навігації)
    if (typeof MutationObserver !== 'undefined') {
        const observer = new MutationObserver(function(mutations) {
            // Перевіряємо тільки якщо зміни не стосуються самого контейнера снігу
            let shouldCheck = false;
            for (let i = 0; i < mutations.length; i++) {
                const mutation = mutations[i];
                if (mutation.target && mutation.target.id !== 'snow-container') {
                    shouldCheck = true;
                    break;
                }
            }
            if (shouldCheck) {
                debouncedEnsure();
            }
        });
        
        // Спостерігаємо за змінами в body
        if (document.body) {
            observer.observe(document.body, {
                childList: true,
                subtree: false
            });
        } else {
            // Якщо body ще не готовий, чекаємо
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', function() {
                    if (document.body) {
                        observer.observe(document.body, {
                            childList: true,
                            subtree: false
                        });
                    }
                });
            }
        }
    }
})(); 