$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return '';
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

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

        $scope.ListaOutros = [];
        $scope.ModoConsulta = false;
        $scope.ShowManut = false;
        $scope.MsgOK = "";
        $scope.MsgNOK = "";
        $scope.Erros = [];
        $scope.MSGPedido = "";
        $scope.ListaItens = [];
        $scope.ReverseRec = false;
        $scope.ProcurarRec = "";
        $scope.Procurar = "";
        $scope.ListaIndicadorPessoa = [{ Value: 1, Text: "CPF" }, { Value: 2, Text: "CNPJ" }];

        $scope.BuscaPessoa = function (tipo) {

            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Pessoa/ListaPessoas")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaPessoas = data;

                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: false,
                        keyboard: true,
                        templateUrl: '../../page/Pessoa/consulta.html',
                        controller: ControllerPessoa,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: $scope.ListaPessoas }); // pass name to Dialog
                    }

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function (value) {

                        if (tipo == "Transportadora") {
                            $scope.transp.cnpjCPF = value.cnpj.replace(".", "").replace(".", "").replace(".", "").replace("-", "").replace("/", "");
                            if ($scope.transp.cnpjCPF.length == 14)
                                $scope.transp.indicadorCnpjCpf = 2;
                            else
                                $scope.transp.indicadorCnpjCpf = 1;

                            $scope.transp.nomeRazao = value.nome;
                            $scope.transp.EnderecoCompleto = value.endereco + "," + value.numero + " " + value.complemento;
                            $scope.transp.cidade = value.cidade;
                            $scope.transp.uf = value.uf;
                            $scope.transp.IE = value.inscricao;
                        }

                        if (tipo == "Tomador") {

                            $scope.NotaFiscalPessoaTomador.cnpjCpf = value.cnpj.replace(".", "").replace(".", "").replace(".", "").replace("-", "").replace("/", "");
                            if ($scope.NotaFiscalPessoaTomador.cnpjCpf.length == 14)
                                $scope.NotaFiscalPessoaTomador.indicadorCnpjCpf = 2;
                            else
                                $scope.NotaFiscalPessoaTomador.indicadorCnpjCpf = 1;

                            $scope.NotaFiscalPessoaTomador.tipoEndereco = value.tipoEndereco;
                            $scope.NotaFiscalPessoaTomador.razao = value.nome;
                            $scope.NotaFiscalPessoaTomador.endereco = value.endereco;
                            $scope.NotaFiscalPessoaTomador.numero = value.numero;
                            $scope.NotaFiscalPessoaTomador.bairro = value.bairro;
                            $scope.NotaFiscalPessoaTomador.cidade = value.cidade;
                            $scope.NotaFiscalPessoaTomador.uf = value.uf;
                            $scope.NotaFiscalPessoaTomador.inscricaoEstadual = value.inscricao;
                            $scope.NotaFiscalPessoaTomador.inscricaoMunicipal = value.ccm;
                            $scope.NotaFiscalPessoaTomador.cep = value.cep;
                            $scope.NotaFiscalPessoaTomador.email = value.eMail;
                            $scope.NotaFiscalPessoaTomador.complemento = value.complemento;
                            $scope.ov.pessoas_ID = value.id;

                        }

                    }, function () {
                        $scope.Pesquisa();
                    });

                }).error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";
                    Metronic.unblockUI();
                });
        }

        $scope.BuscaOV = function () {
            $scope.ModoConsulta = false;
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../OD/ListaOV")
                .success(function (data) {
                    $scope.ListaOV = data;

                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: false,
                        keyboard: true,
                        templateUrl: '../../page/OV/consulta.html',
                        controller: ControllerOV,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: $scope.ListaOV }); // pass name to Dialog
                    }

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function (value) {
                        $scope.ObterPorOV(value.id);

                    }, function () {
                        $scope.Pesquisa();
                    });

                }).error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";

                    Metronic.unblockUI();
                });
        }




        $scope.Filtro = {
            banco_id: "",
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format(),
            valorIni: 0,
            valorFim: 9999999.99
        };

        $scope.Ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        }

        $scope.Voltar = function () {
            $scope.Pesquisa();
            $scope.ShowManut = false;
            $scope.MsgOK = "";
            $scope.MsgNOK = "";
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.id);
        }

        
        $scope.Salvar = function () {

            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalSalvar.html',
                controller: ControllerSalvar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: { 'outroItems': $scope.ListaItens, 'ov': $scope.ov, 'nfp': $scope.NotaFiscalPessoaTomador, 'nf': $scope.nf } }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.showOK = true;
                $scope.MsgOK = data.DSMessage;
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 18000);
            }, function () {
            });


        }

        $scope.Cancelar = function () {
            $scope.ShowManut = false;
            $scope.ModoConsulta = false;
        };



        $scope.LoadListaUnidades = function () {
            $http.post("../../CG/ObterUnidades")
                .success(function (data) {
                    $scope.ListaUnidades = data;
                });
        };
        $scope.LoadListaUnidades();


        $scope.LoadListaBancos = function () {
            $http.post("../../CG/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadListaBancos();

        $scope.Pesquisa = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../OD/ObterTodos/", $scope.filtro2)
            .success(function (data) {
                $scope.ListaOutros = data;
                Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";
                Metronic.unblockUI();
            });
        }

        $scope.ObterPorId = function (id) {
            $scope.ShowManut = true;

            $http.get("../../OD/ObterPorId/" + id)
            .success(function (data) {
                $scope.ov = data.OV;
                $scope.ListaItens = data.Itens;
                $scope.nf = data.NF;
                $scope.NotaFiscalPessoaTomador = data.NotaFiscalPessoaTomador;
            }).error(function (data) {
                $scope.MsgNOK = "Sistema indisponivel";
            });
        }

        $scope.ObterPorOV = function (id) {
            $scope.ShowManut = true;

            $http.get("../../OD/ObterPorOV/" + id)
                .success(function (data) {
                    $scope.ov = data.OV;
                    $scope.ListaItens = data.Itens;
                    $scope.nf = data.NF;
                    $scope.NotaFiscalPessoaTomador = data.NotaFiscalPessoaTomador;
                }).error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";
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
                $scope.Pesquisa();
            }, function () {
            });
        }

        $scope.pedido = { "Valor": 0, "Descricao": "", "Codigo": "", "unidadenegocio_id" : 0 };

        $scope.AdicionarPedido = function () {
            
            $scope.MSGPedido = "";
            if ($scope.pedido.unidadenegocio_id === 0) {
                $scope.MSGPedido = "Informe aq unidade de negocio";
                return;
            }
            if ($scope.pedido.Codigo === "") {
                $scope.MSGPedido = "Informe o Codigo";
                return;
            }
            if ($scope.pedido.Valor === 0) {
                $scope.MSGPedido = "Informe o Valor do Pedido";
                return;
            }
            

            for (var i = 0; i < $scope.ListaUnidades.length; i++) {
                if ($scope.ListaUnidades[i].Value == $scope.pedido.unidadenegocio_id) {
                    $scope.pedido.unidadedesc = $scope.ListaUnidades[i].Text;
                    break;
                }
            }
            $scope.ListaItens.push($scope.pedido);

            $scope.pedido = { "Valor": 0, "Descricao": "", "Codigo": "", "unidadenegocio_id": 0 };

            


            $scope.ov.valor = 0;
            for (var i = 0; i < $scope.ListaItens.length; i++) {
                $scope.ov.valor += $scope.ListaItens[i].Valor;
            }
            $scope.ov.valor = parseFloat($scope.ov.valor.toFixed(2));
        }

        $scope.ExcluirPedido = function (item) {
            Metronic.blockUI({
                boxed: true
            });
            var index = $scope.ListaItens.indexOf(item);
            $scope.ListaItens.splice(index, 1);
            $scope.ov.valor = 0;
            for (var i = 0; i < $scope.ListaItens.length; i++) {
                $scope.ov.valor += $scope.ListaItens[i].Valor;
            }
            $scope.ov.valor = parseFloat($scope.ov.valor.toFixed(2));
            Metronic.unblockUI();
        }

        $scope.Pesquisa();

    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../OD/Excluir", { id: $scope.item.id })
            .success(function (data) {
                Metronic.unblockUI();
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.MsgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
            }).error(function (data) {
                Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.MsgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;



    $scope.OK = function () {

        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../OD/Salvar", $scope.entidade )
            .success(function (data) {
                Metronic.unblockUI();
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.MsgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
            }).error(function (data) {
                Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.MsgNOK = "Sistema indisponivel";
        });
    }

    $scope.Cancelar = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerPessoa = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.ListaPessoas = item.item;
    $scope.reverse = false;
    $scope.showOK = false;
    $scope.showNOK = false;

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
    Metronic.unblockUI();
}
var ControllerOV = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.ListaPessoas = item.item;
    $scope.reverse = false;
    $scope.showOK = false;
    $scope.showNOK = false;

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
    Metronic.unblockUI();
}