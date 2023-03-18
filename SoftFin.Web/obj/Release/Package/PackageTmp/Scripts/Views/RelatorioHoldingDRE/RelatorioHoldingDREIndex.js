$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});
var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination', 'chart.js']);
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

app.filter('SA', function () {
    return function (input) {
        return input == "S" ? 'Sintético' : 'Analítico';
    }
})

app.config(function (ChartJsProvider) {
    // Configure all charts
    ChartJsProvider.setOptions({
        colors: ['#97BBCD', '#DCDCDC', '#F7464A', '#46BFBD', '#FDB45C', '#949FB1', '#4D5360']
    });

});

app.controller('MestreEntidade', [
    '$scope', '$sce', '$http', '$location', '$anchorScroll', '$timeout', '$modal',
    function ($scope, $sce, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.ListaOpcao = [
            { Value: "1", Text: "Sem Ordems" },
            { Value: "2", Text: "Com Ordems" }
        ];
        $scope.reverse = false;
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.filtro = {
            opcao: "1",
            todos: true,
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format()
        };


        $scope.Grafico = function (data) {
            $scope.labelsRecebimento = [];
            $scope.dataRecebimentos = [];
            $scope.labelsDebito = [];
            $scope.dataDebito = [];
            $scope.options = {
                responsive: true,
                legend: {
                    display: true
                },
                tooltips: {
                    enabled: true
                },
                animation: {
                    duration: 30
                },
            };
        }
        $scope.Grafico();

        $scope.Pesquisar = function () {
            Metronic.blockUI({
                boxed: true
            });

            
            estabs = [];

            for (var i = 0; i < $scope.Empresas.length; i++) {
                if ($scope.Empresas[i].Selecionado)
                    estabs.push($scope.Empresas[i].Valor);
            }

            $http.post("../../RelatorioHoldingDRE/ObterRelatorio",
                {
                    "dataInicial": $scope.filtro.dataIni,
                    "dataFinal": $scope.filtro.dataFim,
                    "opc": $scope.filtro.opcao,
                    "estabs": ($scope.filtro.todos == true) ? [] : estabs
                })
                .success(function (data) {
                    $scope.Lista = data.List;
                    
                    $scope.labelsRecebimento = data.Grafico.labelsRecebimento;
                    $scope.dataRecebimentos = data.Grafico.dataRecebimentos;
                    $scope.labelsDebito = data.Grafico.labelsDebito;
                    $scope.dataDebito = data.Grafico.dataDebito;
                    Metronic.unblockUI();
                }).error(function (data) {
                    alert("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }




        $scope.Empresas = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../CG/ObterEstabs")
                .success(function (data) {
                    $scope.Empresas = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.Empresas();

        $scope.Detalhar = function (item) {
            

            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalDetalhe.html',
                controller: ControllerDetalhe,
                resolve: {} // empty storage
            };

            estabs = [];

            for (var i = 0; i < $scope.Empresas.length; i++) {
                if ($scope.Empresas[i].Selecionado)
                    estabs.push($scope.Empresas[i].Valor);
            }

            $scope.opts.resolve.item = function () {
                return angular.copy({
                    item:
                        {
                            "dataInicial": $scope.filtro.dataIni,
                            "dataFinal": $scope.filtro.dataFim,
                            "opcao": $scope.filtro.opcao,
                            "conta": item.codigoConta,
                            "historico": item.descricaoConta,
                            "estabs": ($scope.filtro.todos == true) ? [] : estabs
                        }
                });
                    

            }
            console.log($scope.opts.resolve.item);
            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
            }, function () {
            });

        }
    }]);




var ControllerDetalhe = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.procurar = "";
    $scope.item = item.item;
    Metronic.blockUI({
        boxed: true
    });

    $scope.Pesquisar = function () {



        $http.post("../../RelatorioHoldingDRE/Detalhe",
            {
                "dataInicial": $scope.item.dataInicial,
                "dataFinal": $scope.item.dataFinal,
                "opc": $scope.item.opcao,
                "conta": $scope.item.conta,
                "estabs": $scope.item.estabs, 
            })
            .success(function (data) {
                $scope.objs = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
    }

    $scope.Pesquisar();

    $scope.ordenar = function (keyname) {
        $scope.sortKey = keyname;
        $scope.reverse = !$scope.reverse;
    };

    Metronic.blockUI({
        boxed: true
    });
    $scope.Selecionar = function (value) {
        $modalInstance.close(value);
    }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

}