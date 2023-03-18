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
        $scope.ListaOpcao = [
            { Value: "T", Text: "Todas" },
            { Value: "A", Text: "Aprovadas" },
            { Value: "NA", Text: "Não Aprovadas" }
        ];
        $scope.ListaStatus = [
            { Value: 1, Text: "Liberada" },
            { Value: 2, Text: "Emitida" },
            { Value: 4, Text: "Pendente" },
            { Value: 5, Text: "Cancelada" },
            { Value: 6, Text: "Manual" }
        ];
        $scope.reverse = false;
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.filtro = {
            opcao: "T",
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format()
        };
        $scope.Pesquisar = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../RelatorioOV/ObterRelatorio",
                {
                    "dataIni": $scope.filtro.dataIni, "dataFim": $scope.filtro.dataFim, "tipo": $scope.filtro.opcao, "situacao": $scope.filtro.situacao })
                .success(function (data) {
                    $scope.Lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
    }]);