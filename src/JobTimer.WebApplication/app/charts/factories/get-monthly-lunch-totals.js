var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .factory("getMonthlyLunchTotals", [function () {
                var today = moment();
                var options = {
                    chart: {
                        type: "bar"
                    },
                    title: {
                        text: "Total lunch hours monthly"
                    },
                    subtitle: {
                        text: "Current month: " + today.format("MMMM")
                    },
                    xAxis: {},
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
                        formatter: function () {
                            var currentDay = this.point.x;
                            var lowDate = moment(this.point.low);
                            var day = moment().startOf("month").add(currentDay, "day").format("dddd");
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
                };
                return {
                    options: options,
                    url: "api/chart/getmonthlylunchtotals"
                };
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
