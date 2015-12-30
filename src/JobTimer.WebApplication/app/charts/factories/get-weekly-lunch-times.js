var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .factory("getWeeklyLunchTimes", [function () {
                var today = moment();
                var min = today.startOf("day").add(6, "hour").valueOf();
                var max = today.startOf("day").add(24, "hour").valueOf();
                var options = {
                    chart: {
                        type: "columnrange",
                        inverted: true
                    },
                    title: {
                        text: "Enter and Exit from lunch weekly (first enter and last exit only)"
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
                        formatter: function () {
                            var low = moment(this.point.low).utc().format("HH:mm");
                            var high = moment(this.point.high).utc().format("HH:mm");
                            return ("<b>" + this.point.category + "</b>: " + low + " -> " + high);
                        }
                    },
                    plotOptions: {
                        columnrange: {
                            dataLabels: {
                                enabled: true,
                                formatter: function () {
                                    var d = moment(this.y).utc().format("HH:mm");
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
                    url: "api/chart/getweeklylunchtimes"
                };
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
