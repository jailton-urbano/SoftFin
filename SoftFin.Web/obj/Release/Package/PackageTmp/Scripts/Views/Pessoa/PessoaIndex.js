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
        $http.post("../../Pessoa/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        $scope.ListaNotas = [];
        $scope.idSelecionado = 0
        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showImportacao = false;
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.GERACPAG = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        $scope.ModoConsulta = false;



        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.item = [];
        $scope.pessoa = [];
        $scope.nf = [];
        $scope.CPAG = [];
        $scope.CPAGITEMS = [];




        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.LoadPessoa(0);
        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.LoadListaPessoas();
        }

        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadPessoa(item.id);

        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadPessoa(item.id);

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
            $scope.ListaNotasFiscais();
        }


        $scope.Importar = function (msg) {
            window.location = '../../Pessoa/Import';

        }

        $scope.Excel = function (msg) {
            window.location = '../../Pessoa/Excel';

        }

        $scope.Salvar = function () {

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;

            Metronic.blockUI({
                boxed: true
            });

            var postdata = $scope.pessoa;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };

            $http.post("../../Pessoa/Salvar", { pessoa: postdata, pcc: $scope.Config })
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus == "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        $scope.msgSalvar = "Salvar";
                        $scope.LoadPessoa(postdata.id);

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

        $scope.LoadListaTipoPessoas = function () {

            $http.post("../../Pessoa/ListaTipoPessoas")
                .success(function (data) {
                    $scope.ListaTipoPessoas = data;
                });
        };


        $scope.LoadListaPessoas = function () {

            $http.post("../../Pessoa/ListaPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        };

        $scope.LoadListaCategorias = function () {
            $http.post("../../Pessoa/ListaCategorias")
                .success(function (data) {
                    $scope.ListaCategorias = data;
                });
        }

        $scope.LoadListaUnidades = function () {
            $http.post("../../Pessoa/ListaUnidades")
                .success(function (data) {
                    $scope.ListaUnidades = data;
                });
        }

        $scope.LoadListaTipoEnderecos = function () {
            $http.post("../../Pessoa/ListaTipoEnderecos")
                .success(function (data) {
                    $scope.ListaTipoEnderecos = data;
                });
        }

        $scope.LoadPessoa = function (id) {
            $http.get("../../Pessoa/ObterPessoaPorId/" + id)
                .success(function (data) {
                    $scope.pessoa = data.pessoa;
                    $scope.Config = data.config;
                    $scope.showGrid = false;
                    $scope.showManut = true;
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
                $scope.LoadListaPessoas();
            }, function () {
            });
        }

        $scope.AdicionarFoto = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalAdicionarFoto.html',
                controller: ControllerAdicionarFoto,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadListaPessoas();
            }, function () {
            });
        }


        $scope.ModalContato = function (item) {
            $scope.ModoContratoAlteracao = true;
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalContato.html',
                controller: ControllerContato,
                resolve: {} // empty storage
            };
            if (item == null) {
                $scope.ModoContratoAlteracao = false;
                item = { id: 0, nome: '', cargo: '', email: '', celular: '', observacao: '' }
            }

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {

                if (value != null) {
                    if ($scope.ModoContratoAlteracao) {
                        item.nome = value.nome;
                        item.cargo = value.cargo;
                        item.email = value.email;
                        item.telefone = value.telefone;
                        item.celular = value.celular;
                        item.observacao = value.observacao;
                        item.RecebeCobranca = value.RecebeCobranca;
                    }
                    else
                        $scope.pessoa.contatos.push(value);
                }
            }, function () {
            });
        }

        $scope.ExcluirContato = function (item) {
        }


        $scope.BuscaCEP = function (cep) {


            var ender = "https://viacep.com.br/ws/" + cep.replace('-', '') + "/json/?callback=JSON_CALLBACK";
            $http.jsonp(ender)
                .success(function (data) {
                    $scope.CarregaCEP(data);
                }).error(function () {
                    alert("Pesquisa de CEP Inátiva")
                });
        };



        $scope.BuscaSefaz = function (cpfcnpj) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Pessoa/ConsultaDadosSefaz", { cpfcnpj: cpfcnpj })
                .success(function (data) {
                   
                    Metronic.unblockUI();
                    if (data.CDStatus == "OK") {
                        $scope.pessoa.cep = data.InfConsObj.InfCad.Ender.CEP;
                        $scope.pessoa.nome = data.InfConsObj.XNome;
                        $scope.pessoa.endereco = data.InfConsObj.InfCad.Ender.XLgr;
                        $scope.pessoa.bairro = data.InfConsObj.InfCad.Ender.XBairro;
                        $scope.pessoa.cidade = data.InfConsObj.InfCad.Ender.XMun;
                        //$scope.pessoa.uf = data.InfConsObj.InfCad.Ender.CUF;
                        $scope.pessoa.IE = data.InfConsObj.InfCad.IE;
                        $scope.TipoEndereco();
                    } else {
                        $scope.showNOK = true;
                        $scope.msgNOK = data.DSStatus;
                    }
                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;

                        }, 8000);
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });
        };

        $scope.CarregaCEP = function (data) {
           

            $scope.pessoa.endereco = data.logradouro;
            $scope.pessoa.bairro = data.bairro;
            $scope.pessoa.cidade = data.localidade;
            $scope.pessoa.uf = data.uf;
            $scope.TipoEndereco();


        };

        $scope.TipoEndereco = function () {

            if (data.logradouro.toUpperCase().indexOf("AV") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "1";
            }
            else if (data.logradouro.toUpperCase().indexOf("RUA") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "2";
            }
            else if (data.logradouro.toUpperCase().indexOf("ALAMEDA") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "3";
            }
            else if (data.logradouro.toUpperCase().indexOf("TRAVESSA") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "4";
            }
            else if (data.logradouro.toUpperCase().indexOf("VIA") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "43";
            }
            else if (data.logradouro.toUpperCase().indexOf("ESTRADA") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "44";
            }
            else if (data.logradouro.toUpperCase().indexOf("SE") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "45";
            }
            else if (data.logradouro.toUpperCase().indexOf("ES") >= 0) {
                $scope.pessoa.TipoEndereco_ID = "47";
            }
        }


        $scope.LoadContaContabil = function () {
            $http.get("../../CG/ObterContaContabil")
                .success(function (data) {
                    $scope.ListaContaContabil = data;
                });
        }


        $scope.LoadContaContabil();
        $scope.LoadListaPessoas();
        $scope.LoadListaCategorias();
        $scope.LoadListaUnidades();
        $scope.LoadListaTipoEnderecos();
        $scope.LoadListaTipoPessoas();
    }
]);


var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {


    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Pessoa/Excluir", { id: $scope.item.id })
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMensagem;
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
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


var ControllerAdicionarFoto = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;

    $scope.ok = function () {
        var f = document.getElementById('newPhotos').files[0];
        var fd = new FormData();
        fd.append("file", f);
        fd.append("id", $scope.item.id);


        $http.post("../../../Pessoa/uplodFoto", fd, {
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

    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


var ControllerContato = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.contato = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

    $scope.ok = function () {
        $scope.showOK = true;
        $scope.msgOK = "Salvo com sucesso";

        $timeout(
            function () {
                $scope.showNOK = false;
                $scope.showOK = false;

            }, 8000);

        $modalInstance.close($scope.contato);

    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
