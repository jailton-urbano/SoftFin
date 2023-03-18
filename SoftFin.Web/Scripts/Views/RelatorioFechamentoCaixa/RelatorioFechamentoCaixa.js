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

app.filter('yesNo', function () {
    return function (input) {
        return input === true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.paramReport = "";
        var dataAtual = moment();
        $scope.entidade = { loja: 0, dataFim: dataAtual.format(), dataIni: dataAtual.format(),idCaixa:"", idOperador:""};
        //Funcões Staticas

        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }
        //Funcões Botôes
        $scope.Executar = function () {
            $scope.showOK = false;
            $scope.showNOK = false;

            if ($scope.entidade.loja == "")
            {
                $scope.MsgError("Informe a loja.");
                return;
            }

            $scope.paramReport = "../../RelatorioFechamentoCaixa/Report?idLoja=" + $scope.entidade.loja
                + "&dataIni=" + $scope.entidade.dataIni
                + "&dataFim=" + $scope.entidade.dataFim
                + "&idOperador=" + $scope.entidade.idOperador
                + "&idCaixa=" + $scope.entidade.idCaixa
            ;
        }


        $scope.LoadLoja = function () {
            $http.post("../../RelatorioFechamentoCaixa/ObterLoja")
                .success(function (data) {
                    $scope.ListaLoja = data;
                });
        };

        $scope.LoadCaixa = function () {
            

                Metronic.blockUI({
                    boxed: true
                });

                $http.post("../../RelatorioFechamentoCaixa/ObterCaixa", { id: $scope.entidade.loja })
                    .success(function (data) {
                        $scope.ListaCaixa = data;
                        $scope.LoadOperador($scope.entidade.loja);
                        Metronic.unblockUI();
                    }).error(function (data) {
                        Metronic.unblockUI();
                        $scope.MsgError("Sistema indisponivel");
                    });
            
        };


        $scope.LoadOperador = function (value) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../RelatorioFechamentoCaixa/ObterOperador", { id: value })
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaOperador = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });

        };

        $scope.LoadLoja();

    }]);







