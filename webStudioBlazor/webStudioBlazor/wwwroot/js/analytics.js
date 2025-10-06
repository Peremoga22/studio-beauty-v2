window.GeneralPieChartExpenditure = (analyticsData) => {
    am5.ready(function () {
        // Перевірки підключень
        if (!window.am5) { console.error("amCharts v5 core (index.js) не підключений"); return; }
        if (!window.am5percent) { console.error("amCharts v5 Percent (percent.js) не підключений"); return; }
        if (typeof window.am5themes_Animated === "undefined") {
            console.error("amCharts v5 Theme (themes/Animated.js) не підключений"); return;
        }

        // Якщо графік вже існує — прибираємо
        if (am5.registry.rootElements.length > 0) {
            am5.registry.rootElements.forEach(function (root) {
                if (root.dom && root.dom.id === "chartdiv") root.dispose();
            });
        }

        // 1) Агрегація: сумуємо доходи по категоріях за обраний період
        let totalCosmo = 0, totalMassage = 0;
        for (const p of analyticsData || []) {
            totalCosmo += Number(p.cosmetologyRevenue ?? 0);
            totalMassage += Number(p.massageRevenue ?? 0);
        }
        const pieData = [
            { category: "Cosmetology", value: totalCosmo },
            { category: "Massage", value: totalMassage }
        ];

        // 2) Побудова Pie
        const root = am5.Root.new("chartdiv");
        root.setThemes([am5themes_Animated.new(root)]);

        const chart = root.container.children.push(
            am5percent.PieChart.new(root, {
                // зняв endAngle, щоб це було повне коло (360°)
                innerRadius: 0 // якщо хочеш donut: innerRadius: am5.percent(50)
            })
        );

        const series = chart.series.push(
            am5percent.PieSeries.new(root, {
                valueField: "value",
                categoryField: "category"
            })
        );

        // формат підписів (за бажанням — у грн)
        series.slices.template.setAll({
            tooltipText: "{category}: {value.formatNumber('#,###')}"
        });
        series.labels.template.set("text", "{category}");

        series.data.setAll(pieData);

        // Легенда (опційно)
        const legend = chart.children.push(am5.Legend.new(root, {
            centerX: am5.p50,
            x: am5.p50
        }));
        legend.data.setAll(series.dataItems);

        series.appear(1000, 100);
    });
};
