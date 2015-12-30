namespace JobTimer.Charts {
    angular.module("charts")
        .factory("getWeeklyTimes", [(): IChartOptions => {

            let today = moment();
            let min = today.startOf("day").add(6, "hour").valueOf();
            let max = today.startOf("day").add(24, "hour").valueOf();

            let options: HighchartsOptions =
                {
                    chart: {
                        type: "columnrange",
                        inverted: true
                    },
                    title: {
                        text: "Enter and exit weekly (first enter and last exit only)"
                    },
                    subtitle: {
                        text: "Current week: " + today.format("W")
                    },
                    xAxis: {
                        categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
                    },
                    yAxis: {
                        title: {
                            text: "Time (HH:mm)"
                        },
                        type: "datetime",
                        min: min,
                        max: max
                    },
                    tooltip: {
                        formatter: function() {
                            let low = moment(this.point.low).utc().format("HH:mm");
                            let high = moment(this.point.high).utc().format("HH:mm");
                            return ("<b>" + this.point.category + "</b>: " + low + " -> " + high);
                        }
                    },
                    plotOptions: {
                        columnrange: {
                            dataLabels: {
                                enabled: true,
                                formatter: function() {
                                    let d = moment(this.y).utc().format("HH:mm");
                                    return d;
                                }
                            }
                        }
                    },
                    legend: {
                        enabled: false
                    },
                    series: [
                        {
                            name: "Weekly"
                        }
                    ]
                };
            return {
                options: options,
                url: "api/chart/getweeklytimes"
            };
        }]);
}