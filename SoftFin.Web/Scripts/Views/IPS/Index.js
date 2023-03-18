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
        $http.post("../../IPS/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.ModoConsulta = false;
        $scope.ips = "";

        $scope.showMostraTodosTabela = true;
        $scope.procurarTabela = "";
        $scope.reverseTabela = false;
        $scope.msgSalvarTabela = "Salvar";
        $scope.btnSalvarTabela = true;
        $scope.showGridTabela = true;
        $scope.ModoConsultaTabela = false;
        $scope.tabela = "";

        Metronic.blockUI({
            boxed: true
        });

        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        $scope.ordenarTabela = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverseTabela = !$scope.reverseTabela;
        };

        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaOrigem = [{ Value: "0", Text: "0 - Nacional, exceto as indicadas nos códigos 3 a 5" }
                                , { Value: "1", Text: "1 - Estrangeira - Importação direta, exceto a indicada no código 6;" }
                                , { Value: "2", Text: "2 - Estrangeira - Adquirida no mercado interno, exceto a indicada no código 7;" }
                                , { Value: "3", Text: "3 - Nacional, mercadoria ou bem com Conteúdo de Importação superior a  40% (quarenta por cento);" }
                                , { Value: "4", Text: "4 - Nacional, cuja produção tenha sido feita em conformidade com os processos produtivos básicos de que tratam o Decreto-Lei nº 288/67, e as Leis nºs 8.248/91, 8.387/91, 10.176/01 e 11.484/07;" }
                                , { Value: "5", Text: "5 - Nacional, mercadoria ou bem com Conteúdo de Importação inferior ou igual a 40% (quarenta por cento);" }
                                , { Value: "6", Text: "6 - Estrangeira - Importação direta, sem similar nacional, constante em lista de Resolução CAMEX;" }
                                , { Value: "7", Text: "7 - Estrangeira - Adquirida no mercado interno, sem similar nacional, constante em lista de Resolução CAMEX." }
        ];

        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.LoadIPSporId(0);
        }

        $scope.NovoTabela = function () {
            
            $scope.ModoConsultaTabela = false;
            $scope.LoadTabelaporId(0);
        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            if (!$scope.ModoConsulta)
                $scope.Pesquisa();
        }

        $scope.VoltarTabela = function () {
            $scope.showGridTabela = true;
            $scope.showManutTabela = false;
            if (!$scope.ModoConsultaTabela)
                $scope.PesquisaTabela();
        }

        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadIPSporId(item.id);
        }

        $scope.AlterarTabela = function (item) {
            $scope.ModoConsultaTabela = false;
            $scope.LoadTabelaporId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadIPSporId(item.id);
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
            $scope.LoadIPS();
        }
		    
        $scope.PesquisaTabela = function (msg) {
            $scope.LoadTabela();
        }

        $scope.Excel = function (msg) {
            window.location = '../../IPS/Excel';
        }

        $scope.Salvar = function () {
		        
            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;


            var postdata = $scope.ips;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };

            $http.post("../../IPS/Salvar", postdata)
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
                    $scope.msgSalvar = "Salvar";

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

        $scope.SalvarTabela = function () {
            
            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;


            var postdata = $scope.tabela;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };

            $http.post("../../IPS/SalvarTabela", postdata)
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
                    $scope.msgSalvar = "Salvar";

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
        };

        $scope.LoadCategoria = function () {
            $http.post("../../IPS/ObterCategoria")
                .success(function (data) {
                    $scope.ListaCategoria = data;
                });
        };
        $scope.LoadCategoria();



        $scope.LoadIPS = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../IPS/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.LoadIPS();


        $scope.LoadTabela = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../IPS/ObterTodosTabela/")
            .success(function (data) {
                $scope.listaTabela = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.LoadTabela();

        $scope.LoadIPSporId = function (id) {
            $http.get("../../IPS/ObterPorId/" + id)
            .success(function (data) {
                $scope.ips = data;
                $scope.showGrid = false;
                $scope.showManut = true;
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
            });
        }

        $scope.LoadTabelaporId = function (id) {
            
            $http.get("../../IPS/ObterTabelaPorId/" + id)
            .success(function (data) {
                
                $scope.tabela = data;
                $scope.showGridTabela = false;
                $scope.showManutTabela = true;
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
                $scope.LoadIPS();
            }, function () {
            });
        }

        $scope.ExcluirTabela = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirTabela.html',
                controller: ControllerExcluirTabela,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadTabela();
            }, function () {
            });
        }

        $scope.Tabela = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalRelacionamento.html',
                controller: ControllerRelacionamento,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
               
            }, function () {
            });
        }

    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../IPS/Excluir", { id: $scope.item.id })
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

var ControllerExcluirTabela = function ($scope, $modalInstance, $modal, $http, $timeout, item) {


    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../IPS/ExcluirTabela", { id: $scope.item })
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


var ControllerRelacionamento = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    Metronic.blockUI({
        boxed: true
    });

    $scope.item = item.item;

    $scope.idproduto = $scope.item.id;

    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

    $scope.GerarRelacionamento = function () {
        $http.post("../../IPS/GerarRelacionamento", { itemProdutoServico: $scope.item })
        .success(function (data) {
            $scope.ListaRelacionamentos = data;
            Metronic.unblockUI();
        }).error(function (data) {
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }
    $scope.GerarRelacionamento();

    $scope.ok = function () {
        $http.post("../../IPS/SalvarRelacionamento", { dtoRelaciona: $scope.ListaRelacionamentos, idproduto: $scope.idproduto })
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $modalInstance.close();
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
