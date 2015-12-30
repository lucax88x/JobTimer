var JobTimer;
(function (JobTimer) {
    var Charts;
    (function (Charts) {
        angular.module("charts")
            .factory("getMonthlyTimes", [function () {
                var today = moment();
                var min = today.startOf("day").add(6, "hour").valueOf();
                var max = today.startOf("day").add(24, "hour").valueOf();
                var options = {
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
                        formatter: function () {
                            var low = moment(this.point.low).utc().format("HH:mm");
                            var high = moment(this.point.high).utc().format("HH:mm");
                            var currentDay = parseInt(this.point.category);
                            var lowDate = moment(this.point.low);
                            var day = lowDate.add(-(lowDate.date() - 1), "days").add(currentDay, "days").format("dddd");
                            return ("<b>" + day + " " + (currentDay + 1) + "</b>: " + low + " -> " + high);
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
                            name: "Monthly"
                        }
                    ]
                };
                return {
                    options: options,
                    url: "api/chart/getmonthlytimes"
                };
            }]);
    })(Charts = JobTimer.Charts || (JobTimer.Charts = {}));
})(JobTimer || (JobTimer = {}));
