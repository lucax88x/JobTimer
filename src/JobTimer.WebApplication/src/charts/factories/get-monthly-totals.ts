namespace JobTimer.Charts {
    angular.module("charts")
        .factory("getMonthlyTotals", [(): IChartOptions => {

            let today = moment();

            let options: HighchartsOptions =
                {
                    chart: {
                        type: "bar"
                    },
                    title: {
                        text: "Total monthly"
                    },
                    subtitle: {
                        text: "Current month: " + today.format("MMMM")
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
                    tooltip: {
                        formatter: function() {
                            let currentDay = this.point.x;

                            let lowDate = moment(this.point.low);
                            let day = moment().startOf("month").add(currentDay, "day").format("dddd");

                            return ("<b>" + day + " " + (currentDay + 1) + "</b>");
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
                            name: "Monthly"
                        }
                    ]
                }
            return {
                options: options,
                url: "api/chart/getmonthlytotals"
            };
        }]);
}