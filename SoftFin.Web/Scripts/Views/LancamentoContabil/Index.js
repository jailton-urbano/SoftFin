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

app.filter('debcred', function () {
    return function (input) {
        return input ===  'D' ? 'Débito' : 'Crédito';
    }
})

app.filter('yesNo', function () {
    return function (input) {
        return input === true ? 'Sim' : 'Não';
    }
})

app.filter('filtraGrid', function () {
    return function (input, lista) {
        for (var i = 0; i < lista.length; i++) {
            if (input == lista[i].Value) {
                return lista[i].Text;
            }
        }
        return "-";
    }
});


app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.totDebito = 0;
        $scope.totCredito = 0;
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
        

        $scope.SumGrid = function () {
            $scope.totDebito = 0;
            $scope.totCredito = 0;

            for (var i = 0; i < $scope.entidade.LancamentoContabilDetalhes.length; i++) {
                if ($scope.entidade.LancamentoContabilDetalhes[i].DebitoCredito == 'C') {
                    $scope.totCredito += $scope.entidade.LancamentoContabilDetalhes[i].valor;
                } else {
                    $scope.totDebito += $scope.entidade.LancamentoContabilDetalhes[i].valor;
                }
            }
        }

        //Funcões Staticas
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
            $('#MigralhaC').html("Lista");
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../LancamentoContabil/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.Novo = function () {
            $('#MigralhaC').html("Inclusão");
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        }
        $scope.Voltar = function () {
            $('#MigralhaC').html("Lista");
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
            $scope.ObterTodos();
        }
        $scope.Alterar = function (item) {
            $('#MigralhaC').html("Alteração");
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }
        $scope.Detalhar = function (item) {
            $('#Detalhar').html("Alteração");
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.id);
        }
        $scope.Salvar = function () {
            

            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";
            

            $http.post("../../LancamentoContabil/Salvar", $scope.entidade)
            .success(function (data) {
                Metronic.unblockUI();

                if (data.CDStatus === "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;
                    $location.hash('showOKTop');
                    $scope.msgSalvar = "Salvar";

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

            $http.post("../../LancamentoContabil/ObterPorId", { id: value } )
            .success(function (data) {
                $scope.showGrid = false;
                $scope.showManut = true;
                Metronic.unblockUI();
                $scope.entidade = data;
                $scope.SumGrid();
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




        $scope.NovoDet = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalEditar.html',
                controller: ControllerDetalhe,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                var item = { modo: "I", contaContabil_id: 0, unidadeNegocio_ID: 0, valor: null, DebitoCredito: null, id: 0 };
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.entidade.LancamentoContabilDetalhes.push(value);
                $scope.SumGrid();
            }, function () {
            });
        }

        $scope.AlterarDet = function (item, index) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalEditar.html',
                controller: ControllerDetalhe,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                item.modo = "A";
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.entidade.LancamentoContabilDetalhes.splice(index,1);
                $scope.entidade.LancamentoContabilDetalhes.push(value);
                $scope.SumGrid();
            }, function () {
            });
        }

        $scope.ExcluirDet = function (item, index) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalEditar.html',
                controller: ControllerDetalhe,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                item.modo = "E";
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.entidade.LancamentoContabilDetalhes.splice(index,1);
                $scope.SumGrid();
            }, function () {
            });
        }

        $scope.Arquivo = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalArquivo.html',
                controller: ControllerArquivo,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return "0"; // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
            }, function () {
            });
        }

        $scope.LoadPlanoContas = function () {
            $http.post("../../CG/ObterContaContabil/")
                .success(function (data) {
                    $scope.ListaContaContabil = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        $scope.LoadUnidade = function () {
            $http.post("../../CG/ObterUnidades/")
                .success(function (data) {
                    $scope.ListaUnidade = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }


        $scope.LoadUnidade();
        $scope.LoadPlanoContas();
        $scope.ObterTodos();
    }]);







var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../LancamentoContabil/Excluir", { obj: $scope.item })
        .success(function (data) {
            if (data.CDStatus === "OK") {
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


var ControllerDetalhe = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;
    $scope.ListaDC = [{ Value: "D", Text: "Débito" }, { Value: "C", Text: "Crédito" }];

    $scope.ok = function () {
        //TODO: Validações
        //$scope.showNOK = true;
        //$scope.msgNOK = data.DSMessage;
        //$scope.Erros = data.Erros;
        $scope.showNOK = false;

        if ($scope.entidade.contaContabil_id == 0)
        {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe a conta contabil";
            return;
        }

        if ($scope.entidade.unidadeNegocio_ID == 0) {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe a unidade de negócio";
            return;
        }

        if ($scope.entidade.DebitoCredito == null) {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe Debito ou Crédito";
            return;
        }

        if ($scope.entidade.valor == 0) {
            $scope.showNOK = true;
            $scope.msgNOK = "Informe o valor";
            return;
        }


        $modalInstance.close($scope.entidade);
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };



    $scope.LoadUnidade = function () {
        $http.post("../../CG/ObterUnidades/")
            .success(function (data) {
                $scope.ListaUnidade = data;
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
            });
    }
    $scope.LoadPlanoContas = function () {
        $http.post("../../CG/ObterContaContabil/")
            .success(function (data) {
                $scope.ListaContaContabil = data;
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
            });
    }

    $scope.LoadUnidade();
    $scope.LoadPlanoContas();

}


var ControllerArquivo = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.ini = moment().add(-8, 'days').format();
    $scope.fim = moment().format();
    $scope.tipoarquivo = "";

    $scope.ListaArquivos = [
        { Value: "GerarArquivoDominio", Text: "Arquivo Dominio" },
        { Value: "GerarArquivoProSoft", Text: "Arquivo ProSoft" },
        { Value: "GerarArquivoAdvanced", Text: "Arquivo Advanced" }
    ];

    //$scope.ok = function () {
    //    $http.post("../../LancamentoContabil/" + $scope.tipoarquivo, { obj: $scope.item })
    //        .success(function (data) {
    //            if (data.CDStatus === "OK") {
    //                $modalInstance.close(data);
    //            } else {
    //                $scope.showNOK = true;
    //                $scope.msgNOK = data.DSMessage;
    //                $scope.Erros = data.Erros;
    //            }
    //            $timeout(
    //                function () {
    //                    $scope.showNOK = false;
    //                    $scope.showOK = false;

    //                }, 8000);
    //        }).error(function (data) {
    //            $scope.showNOK = true;
    //            $scope.msgNOK = "Sistema indisponivel";
    //        });
    //}


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}