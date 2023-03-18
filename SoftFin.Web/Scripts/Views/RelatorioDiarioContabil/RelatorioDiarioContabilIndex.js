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
            excel: false
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

        $scope.Search = function () {
            
            Metronic.blockUI({
                boxed: true
            });
            $scope.lista = [];
            if ($scope.filter.excel == false) {
                
                $http.post("../../RelatorioDiarioContabil/ObterRelatorio",
                    {
                        "mes": $scope.filter.mes,
                        "ano": $scope.filter.ano,
                        "excel": $scope.filter.excel
                    })
                    .success(function (data) {
                        $scope.lista = data;
                        Metronic.unblockUI();
                    }).error(function (data) {
                        $scope.MsgError("Sistema indisponivel");
                        Metronic.unblockUI();
                    });
            } else {
                $scope.DownloadExcel();
            }
        };


        $scope.DownloadExcel = function () {
            var mapForm = document.createElement("form");
            mapForm.target = "_self" || "_blank";
            mapForm.id = "stmtForm";
            mapForm.method = "POST";
            mapForm.action = "../../RelatorioFluxoCaixaDiario/ObterRelatorio";

            var mapInput = document.createElement("input");
            mapInput.setAttribute("name", "mes");
            mapInput.setAttribute("value", $scope.filter.mes);
            mapForm.appendChild(mapInput);

            var mapInput2 = document.createElement("input");
            mapInput2.setAttribute("name", "ano");
            mapInput2.setAttribute("value", $scope.filter.ano);
            mapForm.appendChild(mapInput2);

            var mapInput3 = document.createElement("input");
            mapInput3.setAttribute("name", "excel");
            mapInput3.setAttribute("value", true);
            mapForm.appendChild(mapInput3);

            document.body.appendChild(mapForm);

            mapForm.submit();
            Metronic.unblockUI();
        };


    }]);