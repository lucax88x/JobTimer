namespace JobTimer.Charts {
    angular.module("charts")
        .factory("getMonthlyTimes", [(): IChartOptions => {

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
                        text: "Enter and Exit monthly (first enter and last exit only)"
                    },
                    subtitle: {
                        text: "Current Month: " + today.format("MMMM")
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

                            let currentDay = parseInt(this.point.category);

                            let lowDate = moment(this.point.low);
                            let day = lowDate.add(-(lowDate.date() - 1), "days").add(currentDay, "days").format("dddd");

                            return ("<b>" + day + " " + (currentDay + 1) + "</b>: " + low + " -> " + high);
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
                            name: "Monthly"
                        }
                    ]
                }
            return {
                options: options,
                url: "api/chart/getmonthlytimes"
            };
        }]);
}