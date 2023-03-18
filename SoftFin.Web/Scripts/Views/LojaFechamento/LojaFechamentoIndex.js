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

app.filter('flgSituacao', function () {
    return function (input) {
        if (input === "L") {
            return "Lançado";
        }
        if (input === "C") {
            return "Conferido";
        }
        if (input === "F") {
            return "Fechado";
        }
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../LojaFechamento/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });
   

        //Variaveis
        $scope.procurar = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.ModoConsulta = false;
        $scope.ListaSituacao = [{ Value: "L", Text: "Lançado" }, { Value: "C", Text: "Conferido" }, { Value: "F", Text: "Fechado" }];
        //Funcões Staticas

        $scope.EditAccess = false;
        $http.post("../../LojaFechamento/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        //Funcões Botôes
        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../LojaFechamento/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }


        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
            $scope.ObterTodos();
        }


        $scope.Detalhar = function (item) {
            $scope.ListaTipoRecebimentoCaixa = [];
            $scope.ListaCaixa = [];
            $scope.ListaOperador = [];
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.id);
        }

        $scope.Alterar = function (item) {
            $scope.ListaTipoRecebimentoCaixa = [];
            $scope.ListaCaixa = [];
            $scope.ListaOperador = [];
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }

        $scope.Salvar = function () {


            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";

            var entidadeAux = $scope.entidade;
            entidadeAux.LojaFechamentoCCs = $scope.ListaTipoRecebimentoCaixa;


            $http.post("../../LojaFechamento/Salvar", $scope.entidade)
            .success(function (data) {
                Metronic.unblockUI();

                if (data.CDStatus == "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;

                    $scope.msgSalvar = "Salvar";
                    $scope.Voltar();

                } else {
                    $scope.showNOK = true;
                    $scope.Erros = data.Erros;
                    $scope.msgNOK = data.DSMessage;
                    $scope.msgSalvar = "Salvar";

                }

                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";

                    }, 8000);



            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";

                $scope.msgSalvar = "Salvar";

            });
        }
        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../LojaFechamento/ObterPorId", { id: value })
            .success(function (data) {
                $scope.showGrid = false;
                $scope.showManut = true;
                Metronic.unblockUI();
                $scope.entidade = data;

                $scope.LoadCaixa(data.Loja_id);
                $scope.LoadOperador(data.Loja_id);
                $scope.ObterTipoRecebimentoCaixa(data.Loja_id);

            }).error(function (data) {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });
        }

        $scope.Incluir = function (idLoja, dataFechamento) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../LojaFechamento/Incluir", { idLoja: idLoja, dataFechamento: dataFechamento })
            .success(function (data) {

                $scope.showGrid = false;
                $scope.showManut = true;
                Metronic.unblockUI();
                $scope.entidade = data;
                $scope.LoadCaixa(idLoja);
                $scope.LoadOperador(idLoja);
                $scope.ObterTipoRecebimentoCaixa(idLoja);

            }).error(function (data) {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });
        }

        $scope.Excluir = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluir.html',
                controller: ControllerExcluir,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodos();
            }, function () {
            });
        }


        $scope.Novo = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalNovo.html',
                controller: ControllerNovo,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.ListaLoja }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Incluir(value.idLoja, value.dataFechamento);
            }, function () {
            });
        }


        $scope.LoadLoja = function () {
            $http.post("../../LojaFechamento/ObterLoja")
                .success(function (data) {
                    $scope.ListaLoja = data;
                });
        };

        $scope.LoadCaixa = function (value) {
            
            $scope.ListaCaixa = [];

            if (value != null) {
                Metronic.blockUI({
                    boxed: true
                });

                $http.post("../../LojaFechamento/ObterCaixa", { id: value })
                    .success(function (data) {
                        $scope.ListaCaixa = data;
                        Metronic.unblockUI();
                    }).error(function (data) {
                        Metronic.unblockUI();
                        $scope.MsgError("Sistema indisponivel");
                    });
            }
        };

        $scope.ObterTipoRecebimentoCaixa = function (value) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../LojaFechamento/ObterTipoRecebimentoCaixa", { id: $scope.entidade.id, idLoja: value, dataRec: $scope.entidade.dataFechamento })
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaTipoRecebimentoCaixa = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });

        };

        $scope.LoadOperador = function (value) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../LojaFechamento/ObterOperador", { id: value })
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaOperador = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });

        };

        $scope.Calcula = function (value) {

            value.valorTaxa = (value.valorBruto / 100) * value.taxa;
            value.valorLiquido = (value.valorBruto - value.valorTaxa);
            $scope.Saldo();
        };

        $scope.Saldo = function () {


            $scope.entidade.valorLiquido = 0;
            $scope.entidade.valorBruto = 0;
            $scope.entidade.valorTaxas = 0;
            $scope.entidade.saldoFinal = $scope.entidade.saldoInicial;
            angular.forEach($scope.ListaTipoRecebimentoCaixa, function (value) {
                $scope.entidade.valorLiquido += value.valorLiquido;
                $scope.entidade.valorBruto += value.valorBruto;
                $scope.entidade.valorTaxas += value.valorTaxa;
                $scope.entidade.saldoFinal += value.valorLiquido;
            });

        };

        
        

        $scope.LoadLoja();
        $scope.ObterTodos();
    }]);







var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../LojaFechamento/Excluir", { obj: $scope.item })
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}



var ControllerNovo = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.showNOK = false;
    $scope.ListaLoja = item.item;
    var dataAtual = moment();
    $scope.entidade = { idLoja: "", dataFechamento: dataAtual.format() };

    
    
   
    $scope.ok = function () {

        if ($scope.entidade.idLoja == "")
        {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe a loja";
            return;
        }
        
        if ($scope.entidade.dataFechamento == "") {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe a data de Fechamento";
            return;
        }

        $modalInstance.close($scope.entidade);

    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}