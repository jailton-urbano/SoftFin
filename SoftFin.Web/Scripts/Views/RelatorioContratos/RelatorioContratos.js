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
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.filtro = {
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format(),
            excel: false
        };
        $scope.Pesquisar = function () {
            Metronic.blockUI({
                boxed: true
            });
            $scope.Lista = [];
            if ($scope.filtro.excel == false) {
                $http.post("../../RelatorioContratos/ObterRelatorio",
                    {
                        "dataIni": $scope.filtro.dataIni, "dataFim": $scope.filtro.dataFim, "excel": $scope.filtro.excel
                    })
                    .success(function (data) {
                        $scope.Lista = data;
                        Metronic.unblockUI();
                    }).error(function (data) {
                        $scope.MsgError("Sistema indisponivel");
                        Metronic.unblockUI();
                    });
            } else {
                $scope.downloadExcel();
            }
        }


        $scope.downloadExcel = function() {
            var mapForm = document.createElement("form");
            mapForm.target = "_self" || "_blank";
            mapForm.id = "stmtForm";
            mapForm.method = "POST";
            mapForm.action = "../../RelatorioContratos/ObterRelatorio";

            var mapInput = document.createElement("input");
            mapInput.setAttribute("name", "dataIni");
            mapInput.setAttribute("value", $scope.filtro.dataIni);
            mapForm.appendChild(mapInput);

            var mapInput2 = document.createElement("input");
            mapInput2.setAttribute("name", "dataFim");
            mapInput2.setAttribute("value", $scope.filtro.dataFim);
            mapForm.appendChild(mapInput2);

            var mapInput3 = document.createElement("input");
            mapInput3.setAttribute("name", "excel");
            mapInput3.setAttribute("value", true);
            mapForm.appendChild(mapInput3);

            document.body.appendChild(mapForm);

            mapForm.submit();
            Metronic.unblockUI();
        }

        $scope.Unidades = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.get("../../RelatorioContratos/ObterNomesUnidades")
                .success(function (data) {
                    $scope.Unidades = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.Unidades();

    }]);