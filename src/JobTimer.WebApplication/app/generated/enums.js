var ViewModels;
(function (ViewModels) {
    var Master;
    (function (Master) {
        (function (OffsetTypes) {
            OffsetTypes[OffsetTypes["Positive"] = 1] = "Positive";
            OffsetTypes[OffsetTypes["Negative"] = -1] = "Negative";
            OffsetTypes[OffsetTypes["Neutral"] = 0] = "Neutral";
        })(Master.OffsetTypes || (Master.OffsetTypes = {}));
        var OffsetTypes = Master.OffsetTypes;
    })(Master = ViewModels.Master || (ViewModels.Master = {}));
})(ViewModels || (ViewModels = {}));
