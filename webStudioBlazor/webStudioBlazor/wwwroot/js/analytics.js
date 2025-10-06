window.GeneralPieChartExpenditure = (analyticsData) => {
    am5.ready(function () {        
        if (!window.am5) { console.error("amCharts v5 core (index.js) not connect"); return; }
        if (!window.am5percent) { console.error("amCharts v5 Percent (percent.js) not connect"); return; }
        if (typeof window.am5themes_Animated === "undefined") {
            console.error("amCharts v5 Theme (themes/Animated.js) not connect"); return;
        }
                
        if (am5.registry.rootElements.length > 0) {
            am5.registry.rootElements.forEach(function (root) {
                if (root.dom && root.dom.id === "chartdiv") root.dispose();
            });
        }
                
        let totalCosmo = 0, totalMassage = 0;
        for (const p of analyticsData || []) {
            totalCosmo += Number(p.cosmetologyRevenue ?? 0);
            totalMassage += Number(p.massageRevenue ?? 0);
        }
        const pieData = [
            { category: "Косметологія", value: totalCosmo },
            { category: "Масаж", value: totalMassage }
        ];
               
        const root = am5.Root.new("chartdiv");
        root.setThemes([am5themes_Animated.new(root)]);

        const chart = root.container.children.push(
            am5percent.PieChart.new(root, {
                
                innerRadius: 0 //  innerRadius: am5.percent(50)
            })
        );

        const series = chart.series.push(
            am5percent.PieSeries.new(root, {
                valueField: "value",
                categoryField: "category"
            })
        );
                
        series.slices.template.setAll({
            tooltipText: "{category}: {value.formatNumber('#,###')}\u00A0грн"
        });
        series.labels.template.set("text", "{category}");
        series.data.setAll(pieData);
             
        const legend = chart.children.push(am5.Legend.new(root, {
            centerX: am5.p50,
            x: am5.p50
        }));
        legend.data.setAll(series.dataItems);

        series.appear(1000, 100);
    });
};
