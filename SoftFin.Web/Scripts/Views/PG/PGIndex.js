
$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar


    //$('#txtDataDocumento,#txtDatVencimento,#txtDarfDataPagamento,#txtDarfDataPagamento,#txtDataPreVistaPagar,#txtFGTSDataPagamento,#txtdARFDataVencimento').datepicker({
    //    startDate: '-3d'
    //});
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

app.controller('MestreCPAG', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {


        $scope.procurar = "";
        $scope.sortKey = "";
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };


        $scope.EditAccess = false;
        $http.post("../../PG/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        $scope.showGrid = true;
        $scope.ModoConsulta = false;
        $scope.ModoDelete = false;
        $scope.TitulobtnOK = "OK";
        $scope.ShowbtnOK = false;
        $scope.valorTotal = 0;

        $scope.ITEM = {
            'id': 0,
            'ContaContabil': null,
            'UnidadeNegocio': null,
            'valor': 0,
            'PorcentagemLct': 0,
            'historico': null
        };


        $scope.LoadObtemEntidade = function (value) {

            $http.post("../../PG/ObtemEntidade", "{id:" + value.id + "}")
                .success(function (data) {
                    $scope.ITEM = data.objs;

                    $scope.ENTIDADE = {
                        banco_ID: null,
                        historico: value.historico,
                        multa: 0,
                        juros: 0,
                        valorPagamento: 0,
                        dataPagamento: new Date().toISOString(),//JSON.stringify(Date.now()),
                        DocumentoPagarParcela_ID: $scope.ITEM.id,
                        estabelecimento_id: $scope.ITEM.estabelecimento_id
                    };

                    $scope.SomaTotal();
                    $scope.showCadastro = true;
                    $scope.showGrid = false;

                });
        }


        $scope.LoadListaBanco = function () {
            $http.post("../../PG/ListaBanco")
                .success(function (data) {
                    $scope.ListaBanco = data;
                });
        }



        $scope.ConfiguraFiltro = function () {
            $http.get("../../PG/obtemFiltro")
            .success(function (data) {
                $scope.filtro = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                $scope.ListaNotas = [];
                Metronic.unblockUI();
            });
        }

        $scope.ConfiguraFiltro();

        $scope.Adicionar = function (obj) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ShowDivValidacao = false;

            if ($scope.ENTIDADE.banco_ID == null) {
                $scope.MsgDivValidacao = "Informe o banco";
                $scope.ShowDivValidacao = true;
                return;
            }


            if ($scope.ENTIDADE.historico == null) {
                $scope.MsgDivValidacao = "Informe o histórico. ";
                $scope.ShowDivValidacao = true;
                return;
            }

            if ($scope.ENTIDADE.valorPagamento == null) {
                $scope.MsgDivValidacao = "Informe o valor. ";
                $scope.ShowDivValidacao = true;
                return;
            }

            if ($scope.ENTIDADE.valorPagamento == 0) {
                $scope.MsgDivValidacao = "Informe o valor. ";
                $scope.ShowDivValidacao = true;
                return;
            }
            if ($scope.ENTIDADE.contaContabilCredito_id == 0) {
                $scope.MsgDivValidacao = "Informe a Conta Contabil de crédito. ";
                $scope.ShowDivValidacao = true;
                return;
            }
            $http.post('../../PG/Salvar', $scope.ENTIDADE)
            .success(function (data) {
                if (data.CDStatus == "OK") {
                    $scope.ITEM = data.objs;
                    $scope.MsgDivSuccess = 'Salvo com sucesso.';
                } else {
                    $scope.MsgDivDanger = data.CDMessage;
                }
                $scope.SomaTotal();
                Metronic.unblockUI();

            })
            .error(function (data) {
                $scope.MsgDivDanger = 'Sistema Indisponível';
                $scope.SomaTotal();
                Metronic.unblockUI();

            });

            Metronic.blockUI({
                boxed: true
            });


            $scope.SomaTotal();
        }


        $scope.SomaTotal = function () {
            $scope.valorTotal = 0;

            angular.forEach($scope.ITEM.Pagamentos, function (value) {
                console.log(value);
                $scope.valorTotal += value.valorPagamento;
            });
        }




        $scope.Pesquisar = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../PG/ObtemTodos", $scope.filtro)
                .success(function (data) {
                    $scope.ListaPAGAMENTOS = data;
                    $scope.SomaTotal();
                    Metronic.unblockUI();
                });
        }


        $scope.Receber = function (item) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.LoadObtemEntidade(item);
        }

        $scope.Voltar = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.showCadastro = false;
            $scope.showGrid = true;
        }

        Metronic.blockUI({
            boxed: true
        });

        $scope.Excluir = function (item) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";

            Metronic.blockUI({
                boxed: true
            });

            $http.post('../../PG/Excluir', { id: item.id })
            .success(function (data) {
                if (data.CDStatus == "OK") {
                    $scope.ITEM = data.objs;
                    $scope.MsgDivSuccess = 'Excluido com sucesso.';
                } else {
                    $scope.MsgDivDanger = data.CDMessage;
                }
                $scope.SomaTotal();
                Metronic.unblockUI();

            })
            .error(function (data) {
                $scope.MsgDivDanger = 'Sistema Indisponível';
                $scope.SomaTotal();
                Metronic.unblockUI();

            });

        }
        $scope.LoadContaContabil = function () {
            $http.get("../../CG/ObterContaContabil")
                .success(function (data) {
                    $scope.ListaContaContabil = data;
                });
        }

        $scope.Arquivos = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalArquivos.html',
                controller: ControllerArquivos,
                resolve: {} // empty storage
            };



            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {


                //console.log("Modal OK");
            }, function () {
                //on cancel button press
                //console.log("Modal Closed");
            });
        };

        $scope.upload = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalUpload.html',
                controller: ControllerUpload,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                //console.log("Modal OK");
            }, function () {
                //on cancel button press
                //console.log("Modal Closed");
            });
        };

        $scope.LoadContaContabil();
        $scope.ConfiguraFiltro();
        $scope.LoadListaBanco();

    }
]);


var ControllerUpload = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.arquivo = "";

    $scope.someMsg = function () {
        $scope.showNOK = false;
        $scope.showOK = false;
    };

    $scope.ok = function () {
        var descricao = document.getElementById('DescricaoArquivo').value;
        var f = document.getElementById('newPhotos').files[0];
        var fd = new FormData();
        fd.append("file", f);
        fd.append("id", $scope.item.DocumentoPagarMestre_id);
        fd.append("descricao", descricao);
        $http.post("../../../CPAG/Upload", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            };

            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);

            //$modalInstance.close(value);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerArquivos = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;
    console.log($scope.item);
    $scope.carregaArquivos = function () {

        $http.post("../../../CPAG/Arquivos", {
            id: $scope.item.DocumentoPagarMestre_id
         }).success(function (data) {
            $scope.ListaArquivos = data;
            $scope.showOK = true;

        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.Excluir = function (item) {
        $http.post("../../../CPAG/RemoveArquivo", { id: item.DocumentoPagarMestre_id }).success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $scope.carregaArquivos();
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            };

            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);

            //$modalInstance.close(value);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });


    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.carregaArquivos();
}

