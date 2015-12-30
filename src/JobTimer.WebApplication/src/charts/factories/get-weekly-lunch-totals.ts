namespace JobTimer.Charts {
    angular.module("charts")
        .factory("getWeeklyLunchTotals", [(): IChartOptions => {

            let today = moment();

            let options: HighchartsOptions =
                {
                    chart: {
                        type: "bar"
                    },
                    title: {
                        text: "Total lunch hours weekly"
                    },
                    subtitle: {
                        text: "Current week: " + today.format("W")
                    },
                    xAxis: {
                        categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: "Total hours (H.mm)",
                            align: "high"
                        },
                        labels: {
                            overflow: "justify"
                        }
                    },
                    plotOptions: {
                        bar: {
                            dataLabels: {
                                enabled: true
                            }
                        }
                    },
                    legend: {
                        enabled: false
                    },
                    series: [
                        {
                            name: "Weekly"
                        }]
                };
            return {
                options: options,
                url: "api/chart/getweeklylunchtotals"
            };
        }]);
}