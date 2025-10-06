window.GeneralPieChartExpenditure = (analyticsData) => {
    am5.ready(function () {
        try {
            // Перевірки підключень
            if (!window.am5) { console.error("amCharts v5 core (index.js) not connected"); return; }
            if (!window.am5percent) { console.error("amCharts v5 Percent (percent.js) not connected"); return; }
            if (typeof window.am5themes_Animated === "undefined") {
                console.error("amCharts v5 Theme (themes/Animated.js) not connected"); return;
            }

            // Контейнер
            const el = document.getElementById("chartdiv");
            if (!el) { console.error("#chartdiv not found in DOM"); return; }
            // Fallback-висота для мобільних (якщо CSS не спрацював)
            if (el.clientHeight === 0) {
                const w = el.clientWidth || window.innerWidth || 360;
                const isNarrow = w < 576;
                el.style.minHeight = Math.max(220, Math.round(w * (isNarrow ? 0.75 : 0.6))) + "px";
            }

            // Закрити попередній root для цього div
            am5.registry.rootElements
                .filter(r => r.dom && r.dom.id === "chartdiv")
                .forEach(r => r.dispose());

            // Агрегація сум
            let totalCosmo = 0, totalMassage = 0;
            for (const p of (analyticsData || [])) {
                totalCosmo += Number(p.cosmetologyRevenue ?? 0);
                totalMassage += Number(p.massageRevenue ?? 0);
            }
            const pieData = [
                { category: "Косметологія", value: totalCosmo },
                { category: "Масаж", value: totalMassage }
            ];

            // Створюємо root
            const root = am5.Root.new("chartdiv");
            root.setThemes([am5themes_Animated.new(root)]);
            // Формат чисел: укр. локаль, без десяткових
            root.numberFormatter.setAll({ numberFormat: "#,###", intlLocales: "uk-UA" });

            // Якщо даних немає — показати повідомлення і вийти
            if ((totalCosmo + totalMassage) <= 0) {
                root.container.children.push(
                    am5.Label.new(root, {
                        text: "Немає даних за обраний період",
                        centerX: am5.p50, centerY: am5.p50,
                        fontSize: 16, fill: am5.color(0x6c757d)
                    })
                );
                return;
            }

            // Pie chart
            const chart = root.container.children.push(am5percent.PieChart.new(root, {
                // повне коло; для donut: innerRadius: am5.percent(55)
            }));

            const series = chart.series.push(am5percent.PieSeries.new(root, {
                valueField: "value",
                categoryField: "category"
            }));

            // Підписи та підказки без .00 і з "грн"
            series.slices.template.setAll({
                tooltipText: "{category}: {value.formatNumber('#,###')}\u00A0грн ({valuePercentTotal.formatNumber('#,##0')}%)"
            });
            series.labels.template.setAll({
                text: "{category}: {value.formatNumber('#,###')}\u00A0грн",
                // краще для довгих підписів:
                oversizedBehavior: "wrap",
                textType: "circular"
            });

            series.data.setAll(pieData);

            // Легенда
            const legend = chart.children.push(am5.Legend.new(root, {
                centerX: am5.p50, x: am5.p50
            }));
            legend.data.setAll(series.dataItems);

            // Анімація
            series.appear(800, 100);
            chart.appear(800, 100);
        } catch (e) {
            console.error("GeneralPieChartExpenditure error:", e);
        }
    });
};
