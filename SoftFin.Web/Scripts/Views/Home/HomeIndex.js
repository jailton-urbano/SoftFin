
$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

var app = angular.module('SOFTFIN', [
    'ui.bootstrap',
    'ui.utils.masks',
    'angularUtils.directives.dirPagination',
    'ui.calendar'
]);

app.directive('datetimez', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            element.datetimepicker({
                dateFormat: 'dd/MM/yyyy',
                language: 'ptbr',
                pickTime: false,
                startDate: '01-11-2013',      // set a minimum date
                endDate: '01-11-2030'          // set a maximum date
            }).on('changeDate', function (e) {
                ngModelCtrl.$setViewValue(moment(e.localDate).format());
                scope.$apply();
                $(this).datetimepicker('hide');
            });
        }
    };
});




app.controller('MestreCTR', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', '$compile','uiCalendarConfig',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal, $compile, uiCalendarConfig) {
        $scope.uiConfig = {
            calendar: {
                height: 300,
                editable: false,
                header: {
                    left: 'title',
                    center: '',
                    right: ''
                }
            }
        };

        $scope.eventSources = [{
            url: "../../Home/GetCalendar/"
        }];

    }
]);
