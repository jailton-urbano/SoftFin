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
        return input == "True" ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../OV/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        $scope.ListaNotas = [];
        $scope.ListaTipoFaturamento = [{ Value: 0, Text: "Serviço(NFs)" }, { Value: 1, Text: "Mercadoria(NFe)" }, { Value: 2, Text: "Outros" }];
        $scope.idSelecionado = 0
        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        $scope.ModoConsulta = false;

        $scope.filtro = {

            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format()
        };
     
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.ov = [];

        $scope.Novo = function () {
            $scope.ModoConsulta = false;

            $scope.LoadOVporId(0);
        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            if (!$scope.ModoConsulta)
                $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadOVporId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadOVporId(item.id);
        }

        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }

        $scope.MsgSucesso = function (msg) {
            $scope.msgOK = msg;
            $scope.showOK = true;
            $scope.showNOK = false;
        }


        $scope.Pesquisa = function (msg) {
            $scope.LoadOV();
        }


        $scope.Excel = function (msg) {
            window.location = '../../OV/Excel';
        }

        $scope.Salvar = function () {

            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;


            var postdata = $scope.ov;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };

            $http.post("../../OV/Salvar", postdata)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus == "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        $location.hash('showOKTop');
                        $scope.msgSalvar = "Salvar";

                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                            $scope.msgSalvar = "Salvar";
                            $scope.btnSalvar = true;

                        }, 8000);



                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";

                    $scope.msgSalvar = "Salvar";
                    $scope.btnSalvar = true;

                });
        }

        $scope.cancel = function () {
            $scope.showImportacao = false;
            $scope.showMostraTodos = true;
            $location.hash('DivTotal');
        };

        $scope.LoadListaStatus = function (value) {
            $http.post("../../OV/ObterStatus", { ispendente: value })
                .success(function (data) {
                    $scope.ListaStatus = data;
                });
        };


        $scope.LoadListaUnidadeNegocios = function () {
            $http.post("../../OV/ObterUnidadeNegocios")
                .success(function (data) {
                    $scope.ListaUnidadeNegocios = data;
                });
        };
        $scope.LoadListaUnidadeNegocios();

        $scope.LoadListaItemProdutoServicos = function () {
            $http.post("../../OV/ObterItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaItemProdutoServicos = data;
                });
        };
        $scope.LoadListaItemProdutoServicos();

        $scope.LoadListaTabelaPrecoItemProdutoServicos = function () {
            $http.post("../../OV/ObterTabelaPrecoItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaTabelaPrecoItemProdutoServicos = data;
                });
        };
        $scope.LoadListaTabelaPrecoItemProdutoServicos();

        $scope.LoadListaBanco = function () {
            $http.post("../../OV/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadListaBanco();

        $scope.LoadObterPessoas = function () {
            $http.post("../../OV/ObterPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        };
        $scope.LoadObterPessoas();


        $scope.LoadOV = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../OV/ObterTodos/", $scope.filtro)
                .success(function (data) {
                    $scope.lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        //  $scope.LoadOV();

        $scope.LoadOVporId = function (id) {
            $http.get("../../OV/ObterPorId/" + id)
                .success(function (data) {
                    $scope.ov = data;
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    
                    if (data.statusParcela_ID != 4) {
                        $scope.LoadListaStatus(false);
                        $scope.ModoConsulta = true;
                    } else {
                        $scope.LoadListaStatus(true);
                    }


                }).error(function (data) {
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
                $scope.LoadOV();
            }, function () {
            });
        }


    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../OV/Excluir", { id: $scope.item.id })
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
