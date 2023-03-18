$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});
var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);
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

app.filter('removezero', function () {
    return function (input) {
        return input.replace("00:00:00","");
    };
});

app.controller('MestreEntidade', [
    '$scope', '$sce', '$http', '$location', '$anchorScroll', '$timeout', '$modal',
    function ($scope, $sce, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.reverse = false;
        $scope.Order = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.filter = {
            
            mes: 12,
            ano: 2018,
            banco_id: 0
        };

        $scope.listMonth = [
            { Value: 1, Text: "janeiro" },
            { Value: 2, Text: "fevereiro" },
            { Value: 3, Text: "março" },
            { Value: 4, Text: "abril" },
            { Value: 5, Text: "maio" },
            { Value: 6, Text: "junho" },
            { Value: 7, Text: "julho" },
            { Value: 8, Text: "agosto" },
            { Value: 9, Text: "setembro" },
            { Value: 10, Text: "outubro" },
            { Value: 11, Text: "novembro" },
            { Value: 12, Text: "dezembro" }
        ];

        $scope.listYear = [
            { Value: 2017, Text: "2017" },
            { Value: 2018, Text: "2018" },
            { Value: 2019, Text: "2019" }
        ];

        $scope.LoadBancos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../MC/ObterBanco/", $scope.Filter)
                .success(function (data) {
                    $scope.listBancos = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        };
        $scope.LoadBancos();


        $scope.Search = function () {
            
            Metronic.blockUI({
                boxed: true
            });
            $scope.lista = [];
            
                
            $http.post("../../MC/ObterRelatorio",
                {
                    "mes": $scope.filter.mes,
                    "ano": $scope.filter.ano,
                    "banco": $scope.filter.banco_id
                })
                .success(function (data) {
                    $scope.list = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });

        };


      
    }]);