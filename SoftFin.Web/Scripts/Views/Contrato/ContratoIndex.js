
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
        return input === "True" ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.reverse2 = false;
        $scope.reverse3 = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.ModoConsulta = false;
        $scope.showGrid = true;


        $scope.EditAccess = false;
        $http.post("../../Contrato/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ordenar2 = function (keyname) {
            $scope.sortKey2 = keyname;
            $scope.reverse2 = !$scope.reverse2;
        };
        $scope.ordenar3 = function (keyname) {
            $scope.sortKey3 = keyname;
            $scope.reverse3 = !$scope.reverse3;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        
        $scope.LoadListaPessoas = function () {
            $http.post("../../CPAG/ListaPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        };



        $scope.LoadContrato = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            var postdata = {id: value };
            $http.post("../../Contrato/ObterContratosPorId", postdata)
                .success(function (data) {
                    $scope.showGrid = false;
                    Metronic.unblockUI();
                    $scope.contrato = data;
                    $scope.LoadListaContratosItens();
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                    $scope.msgSalvar = "Salvar";
                    $scope.btnSalvar = true

                });
            }
        $scope.LoadListaContratos = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../Contrato/ObterContratos")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaContratos = data;
            });
        };
        $scope.LoadContratoItem = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Contrato/ObterContratoItemsPorId", { id: value })
            .success(function (data) {
                Metronic.unblockUI();
                data.contrato_id = $scope.contrato.id;
                $scope.EditCI(data);
            });
        };
        $scope.LoadListaContratosItens = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Contrato/ObterContratoItems", { id: $scope.contrato.id })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaContratosItens = data;
            });
        };
        $scope.LoadParcela = function (value, contratoitem) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../Contrato/ObterParcelaPorId", { id: value })
            .success(function (data) {
                Metronic.unblockUI();
                data.contratoitem_ID = contratoitem.id;
                $scope.EditPA(data, contratoitem);
            });
        };
        $scope.LoadListaParcelas = function (contratoitem) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../Contrato/ObterParcelas", { id: contratoitem.id })
                .success(function (data) {
                    Metronic.unblockUI();
                    contratoitem.ListaParcelas = data;
                });
        };
        $scope.Incluir = function () {
            $scope.ModoConsulta = false;
            $scope.LoadContrato(0);
        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.LoadListaContratos();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadContrato(item.id);
        }
        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadContrato(item.id);
        }
        $scope.EditCI = function (item) {
            $scope.opts = {
            size: 'lg',
            animation: true,
            backdrop: false,
            backdropClick: false,
            dialogFade: false,
            keyboard: true,
            templateUrl: 'modalContratoItem.html',
            controller: ControllerCI,
            resolve: {} // empty storage
        };

        item.ModoConsultaCI = $scope.ModoConsultaCI;

        $scope.opts.resolve.item = function () {
            return angular.copy({item: item }); // pass name to Dialog
        }


        var modalInstance = $modal.open($scope.opts);

        modalInstance.result.then(function (value) {
            $scope.LoadListaContratosItens();
            }, function () {
            });
        }
        $scope.EditPA = function (item, contratoitem) {

            if (item.id == 0) {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalParcela.html',
                    controller: ControllerPA,
                    resolve: {} // empty storage
                };
            } else {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalParcelaPedido.html',
                    controller: ControllerPAP,
                    resolve: {} // empty storage
                };
            }

            item.ModoConsultaPA = $scope.ModoConsultaPA;

            $scope.opts.resolve.item = function () {
                return angular.copy({item: item }); // pass name to Dialog
            }
            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadListaParcelas(contratoitem);
            }, function () {
            });
        }

        $scope.EditPAMulti = function (item) {
            
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalParcelaMulti.html',
                controller: ControllerPAMulti,
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
        $scope.IncluirCI = function () {
            $scope.ModoConsultaCI = false;
            $scope.LoadContratoItem(0);
        }
        $scope.AlterarCI = function (item) {
            $scope.ModoConsultaCI = false;
            $scope.LoadContratoItem(item.id);
        }
        $scope.DetalharCI = function (item) {
            $scope.ModoConsultaCI = true;
            $scope.LoadContratoItem(item.id);
        }
        $scope.PesquisaCI = function (msg) {
            $scope.ListaContratoItem();
        }
        $scope.IncluirPA = function (contratoitem) {
            $scope.ModoConsultaPA = false;
            $scope.LoadParcela(0, contratoitem);
        }
        $scope.AlterarPA = function (item, contratoitem) {
            $scope.ModoConsultaPA = false;
            $scope.LoadParcela(item.id, contratoitem);
        }
        $scope.DetalhaPA = function (item, contratoitem) {
            $scope.ModoConsultaPA = true;
            $scope.LoadParcela(item.id, contratoitem);
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
        $scope.Salvar = function () {
            Metronic.blockUI({
                boxed: true
            });
            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;
            var postdata = $scope.contrato;
            $http.post("../../Contrato/Salvar", postdata)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus == "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        $scope.msgSalvar = "Salvar";
                        $scope.contrato = data.Obj;
                        $scope.LoadListaContratosItens();
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
                return angular.copy({item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadListaContratos();
            }, function () {
            });
        }
        $scope.ExcluirContratoItem = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirContratoItem.html',
                controller: ControllerExcluirContratoItem,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadListaContratosItens();
            }, function () {
            });
        }
        $scope.ExcluirParcela = function (item, contratoitem) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirParcela.html',
                controller: ControllerExcluirParcela,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
            $scope.LoadListaParcelas(contratoitem);
            }, function () {
            });
        }
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
                return angular.copy({item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
            }, function () {
            });
        };
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
                return angular.copy({item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
            }, function () {
            });
        };


        $scope.BuscaPessoa = function (tipo) {

            Metronic.blockUI({
                boxed: true
            });

            if (tipo == 'Cliente')
                metodo = "ListaClientes";
            else
                metodo = "ListaVendedores";

            $http.post("../../Contrato/" + metodo)
                .success(function (data) {
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

                        if (tipo == 'Cliente') {
                            $scope.contrato.pessoas_ID = value.id;
                            $scope.contrato.pessoa_nome = value.nome + ", " + value.cnpj;
                        }
                        else {
                            $scope.contrato.Vendedor_id = value.id;
                            $scope.contrato.Vendedor_nome = value.nome + ", " + value.cnpj;

                        }
                            

                    }, function () {
 
                    });

                });
        }

        $scope.BuscaMunicipios = function () {

            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Contrato/ListaMunicipios")
                .success(function (data) {
                    $scope.ListaMunicipios = data;

                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: false,
                        keyboard: true,
                        templateUrl: '../../page/Municipio/consulta.html',
                        controller: ControllerMunicipios,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: $scope.ListaMunicipios }); // pass name to Dialog
                    }

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function (value) {
                        $scope.contrato.MunicipioPrestador_id = value.ID_MUNICIPIO;
                        $scope.contrato.MunicipioPrestador_nome = value.DESC_MUNICIPIO;
                    });

                });
        }

        $scope.LoadListaContratos();
        $scope.LoadListaPessoas();
}]);


var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Contrato/Excluir", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();
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

var ControllerExcluirContratoItem = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    
    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Contrato/ExcluirContratoItem", $scope.item)
            .success(function (data) {
                Metronic.unblockUI();

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
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerExcluirParcela = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ExcluirParcelaContrato", $scope.item)
        .success(function (data) {
            Metronic.unblockUI();

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
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
}


        $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
        };
        }

var ControllerCI = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    
    $scope.contratoitem = item.item;
    $scope.ModoConsultaCI = item.item.ModoConsultaCI;
    $scope.reverseRec = false;
    $scope.procurarRec = "";


    //$scope.LoadItemUnidade = function () {

    //    if ($scope.contratoitem.id != 0) {

    //        Metronic.blockUI({
    //            boxed: true
    //        });

    //        $http.post("../../Contrato/ObterItemUnidade", { 'id': $scope.contratoitem.id })
    //            .success(function (data) {
    //                Metronic.unblockUI();
    //                $scope.contratoitem.ListaItemUnidades = data;
    //            }).error(function () {
    //                Metronic.unblockUI();
    //            });
    //    }
    //};
    //$scope.LoadItemUnidade();

    $scope.AdicionarUnidade = function () {

        $scope.showMSGUnidade = false;
        $scope.msgMSGUnidade = "";
        if ($scope.unidade.Unidade === "") {
            $scope.showMSGUnidade = true;
            $scope.msgMSGUnidade = "Informe o Unidade";
            return;
        }
        if ($scope.unidade.Valor === 0) {
            $scope.showMSGUnidade = true;
            $scope.msgMSGUnidade = "Informe o Valor do Unidade";
            return;
        }
        for (var i = 0; i < $scope.ListaUnidadeNegocios.length; i++) {
            if ($scope.ListaUnidadeNegocios[i].Value == $scope.unidade.UnidadeNegocio_Id) {
                $scope.unidade.Unidade_Desc = $scope.ListaUnidadeNegocios[i].Text;
            }
        }

        $scope.contratoitem.ListaItemUnidades.push($scope.unidade);
        $scope.NovaItem();
        $scope.SomaValor();
    };

    $scope.ExcluirUnidade = function (item) {
        Metronic.blockUI({
            boxed: true
        });
        var index = $scope.contratoitem.ListaItemUnidades.indexOf(item);
        $scope.contratoitem.ListaItemUnidades.splice(index, 1);
        $scope.SomaValor();
        Metronic.unblockUI();
    }



    $scope.SomaValor = function () {
        $scope.contratoitem.valor = 0;
        for (var i = 0; i < $scope.contratoitem.ListaItemUnidades.length; i++) {
            $scope.contratoitem.valor += $scope.contratoitem.ListaItemUnidades[i].Valor;
        }
    }   

    $scope.NovaItem = function () {
        $scope.unidade = { "Valor": 0, "Descricao": "", "UnidadeNegocio_Id": "", "Id": 0 };
    }
    $scope.NovaItem();
    $scope.LoadTipoContratos = function () {

        $http.post("../../Contrato/ObterTipoContratos")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaTipoContratos = data;
            });
    };

    $scope.LoadUnidadeNegocio = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Contrato/ObterUnidadeNegocios")
            .success(function (data) {

                Metronic.unblockUI();
                $scope.ListaUnidadeNegocios = data;
            });
    };


        $scope.LoadItemProdutoServicos = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../Contrato/ObterItemProdutoServicos")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaProdutoServicos = data;
                });
        };

        $scope.LoadTabelaPrecoItemProdutoServicos = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../Contrato/ObterTabelaPrecoItemProdutoServicos")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaTabelaPrecos = data;
                });
        };


        $scope.ok = function () {
            Metronic.blockUI({
                boxed: true
            });


            $http.post("../../Contrato/SalvarContratoItem", { 'obj': $scope.contratoitem, 'listaItemUnidades': $scope.contratoitem.ListaItemUnidades })
                .success(function (data) {
                    Metronic.unblockUI();

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

                    Metronic.unblockUI();

                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });

        }
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
        $scope.LoadTipoContratos();
        $scope.LoadItemProdutoServicos();
        $scope.LoadUnidadeNegocio();
        $scope.LoadTabelaPrecoItemProdutoServicos();
    }


var ControllerPA = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    Metronic.blockUI({
        boxed: true
    });

    $scope.parcela = item.item;
    $scope.ModoConsultaPA = item.item.ModoConsultaPA;
    $scope.ListaSimNao = [{Value: false, Text: "Não" }, {Value: true, Text: "Sim" }];
    $scope.ListaTipoFaturamento = [{ Value: 0, Text: "Serviço(NFs)" }, { Value: 1, Text: "Mercadoria(NFe)" }, { Value: 2, Text: "Outros" }];

    $scope.LoadBancos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterBanco")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaBancos = data;
        });
    };

    $scope.LoadCodigoServicos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterCodigoServicos")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaCodigoServicos = data;
            });
    };

    $scope.LoadOperacoes = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterOperacoes")
        .success(function (data) {
            Metronic.unblockUI();
            $scope.ListaOperacoes = data;
        });
    };



    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/SalvarParcela", $scope.parcela)
            .success(function (data) {
            Metronic.unblockUI();

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
            Metronic.unblockUI();

            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.LoadBancos();
    $scope.LoadCodigoServicos();
    $scope.LoadOperacoes();

}

var ControllerPAP = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    Metronic.blockUI({
        boxed: true
    });

    $scope.parcela = item.item;
    $scope.ModoConsultaPA = item.item.ModoConsultaPA;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.ListaTipoFaturamento = [{ Value: 0, Text: "Serviço(NFs)" }, { Value: 1, Text: "Mercadoria(NFe)" }, { Value: 2, Text: "Outros" }];

    $scope.LoadBancos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterBanco")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaBancos = data;
            });
    };

    $scope.LoadPedidos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterContratoItemPedido", { "Id": $scope.parcela.id })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaContratoItemPedido = data;

                if ($scope.somaaposcarga) {
                    $scope.parcela.valor = 0;
                    for (var i = 0; i < $scope.ListaContratoItemPedido.length; i++) {
                        $scope.parcela.valor += $scope.ListaContratoItemPedido[i].Valor;
                    }
                }
            });
    };

    $scope.LoadCodigoServicos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterCodigoServicos")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaCodigoServicos = data;
            });
    };

    $scope.LoadOperacoes = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterOperacoes")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaOperacoes = data;
            });
    };


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $scope.procurar = "";


        $http.post("../../Contrato/SalvarParcela", { "obj": $scope.parcela, "pedidos": $scope.ListaContratoItemPedido})
            .success(function (data) {
                Metronic.unblockUI();

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
                Metronic.unblockUI();

                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.AdicionarPedido = function () {

        $scope.showMSGPedido = false;
        $scope.MSGPedido = "";
        if ($scope.pedido.unidadenegocio_id === 0) {
            $scope.MSGPedido = "Informe a unidade de negocio";
            return;
        }
        $scope.msgMSGPedido = "";
        if ($scope.pedido.Pedido === "") {
            $scope.showMSGPedido = true;
            $scope.msgMSGPedido = "Informe o Pedido";
            return; 
        }
        if ($scope.pedido.Valor === 0) {
            $scope.showMSGPedido = true;
            $scope.msgMSGPedido = "Informe o Valor do Pedido";
            return;
        }
        for (var i = 0; i < $scope.ListaUnidades.length; i++) {
            if ($scope.ListaUnidades[i].Value == $scope.pedido.unidadenegocio_id) {
                $scope.pedido.unidadedesc = $scope.ListaUnidades[i].Text;
                break;
            }
        }
        $scope.ListaContratoItemPedido.push($scope.pedido );
        $scope.pedido = { "Valor": 0, "Descricao": "", "Pedido": "", "unidadenegocio_id": 0  };
        $scope.parcela.valor = 0;
        for (var i = 0; i < $scope.ListaContratoItemPedido.length; i++) {
            $scope.parcela.valor += $scope.ListaContratoItemPedido[i].Valor;
        }
    }

    $scope.ExcluirPedido = function (item) {
        Metronic.blockUI({
            boxed: true
        });
        var index = $scope.ListaContratoItemPedido.indexOf(item);
        $scope.ListaContratoItemPedido.splice(index, 1);
        $scope.parcela.valor = 0;
        for (var i = 0; i < $scope.ListaContratoItemPedido.length; i++) {
            $scope.parcela.valor += $scope.ListaContratoItemPedido[i].Valor;
        }
        Metronic.unblockUI();
    }


    $scope.ImportaPedidos = function () {
        Metronic.blockUI({
            boxed: true
        });
        $scope.showMSGUpPedido = false;
        var f = document.getElementById('ArquivoCSV').files[0];
        var fd = new FormData();
        fd.append("file", f);
        fd.append("Id", $scope.parcela.id );


        $http.post("../../../Contrato/ImportCSVPedido", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            Metronic.unblockUI();

            if (data.Situacao == "OK") {
                $scope.somaaposcarga = true;
                $scope.LoadPedidos();


            } else {
                $scope.showMSGUpPedido = true;
                $scope.msgMSGUpPedido = importacaoArquivo.Situacao;
            };


        }).error(function (data) {
            Metronic.unblockUI();

            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
        $scope.pedido = { "Valor": 0, "Descricao": "", "Pedido": "", "unidadenegocio_id": 0  };
    $scope.ListaContratoItemPedido = [];
    $scope.showMSGPedido = false;
    $scope.showMSGUpPedido = false;
    $scope.msgMSGPedido = "";
    $scope.msgMSGUpPedido = "";
    $scope.LoadBancos();
    $scope.LoadCodigoServicos();
    $scope.LoadOperacoes();
    $scope.somaaposcarga = false;
    $scope.LoadPedidos();
    $scope.LoadListaUnidades = function () {
        $http.post("../../CG/ObterUnidades")
            .success(function (data) {
                $scope.ListaUnidades = data;
            });
    };
    $scope.LoadListaUnidades();
}

var ControllerPAMulti = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    
    Metronic.blockUI({
        boxed: true
    });

    $scope.LoadListaParcelas = function (contratoitem) {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../Contrato/ObterParcelasEmAberto", { id: contratoitem.id })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.Lista = data;
            });
    };

    $scope.CopiaResultado = function (index, conteudo, campo) {
        
        for (var i = index; i < $scope.Lista.length; i++) {
            //if (campo == 'descricao')
            var campoaux = $scope.Lista[i];
            campoaux[campo] = conteudo;
        }

        
    };



    $scope.LoadListaParcelas(item.item); 
    

    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.ListaTipoFaturamento = [{ Value: 0, Text: "Serviço(NFs)" }, { Value: 1, Text: "Mercadoria(NFe)" }, { Value: 2, Text: "Outros" }];

    $scope.LoadBancos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterBanco")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaBancos = data;
            });
    };



    $scope.LoadCodigoServicos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterCodigoServicos")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaCodigoServicos = data;
            });
    };

    $scope.LoadOperacoes = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Contrato/ObterOperacoes")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaOperacoes = data;
            });
    };


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });
        $scope.procurar = "";


        $http.post("../../Contrato/SalvarParcelas", { "objs": $scope.Lista})
            .success(function (data) {
                Metronic.unblockUI();

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
                Metronic.unblockUI();

                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.AdicionarPedido = function () {

        $scope.showMSGPedido = false;
        $scope.MSGPedido = "";
        if ($scope.pedido.unidadenegocio_id === 0) {
            $scope.MSGPedido = "Informe a unidade de negocio";
            return;
        }
        $scope.msgMSGPedido = "";
        if ($scope.pedido.Pedido === "") {
            $scope.showMSGPedido = true;
            $scope.msgMSGPedido = "Informe o Pedido";
            return;
        }
        if ($scope.pedido.Valor === 0) {
            $scope.showMSGPedido = true;
            $scope.msgMSGPedido = "Informe o Valor do Pedido";
            return;
        }
        for (var i = 0; i < $scope.ListaUnidades.length; i++) {
            if ($scope.ListaUnidades[i].Value == $scope.pedido.unidadenegocio_id) {
                $scope.pedido.unidadedesc = $scope.ListaUnidades[i].Text;
                break;
            }
        }
        $scope.ListaContratoItemPedido.push($scope.pedido);
        $scope.pedido = { "Valor": 0, "Descricao": "", "Pedido": "", "unidadenegocio_id": 0 };
        $scope.parcela.valor = 0;
        for (var i = 0; i < $scope.ListaContratoItemPedido.length; i++) {
            $scope.parcela.valor += $scope.ListaContratoItemPedido[i].Valor;
        }
    }

    $scope.ExcluirPedido = function (item) {
        Metronic.blockUI({
            boxed: true
        });
        var index = $scope.ListaContratoItemPedido.indexOf(item);
        $scope.ListaContratoItemPedido.splice(index, 1);
        $scope.parcela.valor = 0;
        for (var i = 0; i < $scope.ListaContratoItemPedido.length; i++) {
            $scope.parcela.valor += $scope.ListaContratoItemPedido[i].Valor;
        }
        Metronic.unblockUI();
    }


    $scope.ImportaPedidos = function () {
        Metronic.blockUI({
            boxed: true
        });
        $scope.showMSGUpPedido = false;
        var f = document.getElementById('ArquivoCSV').files[0];
        var fd = new FormData();
        fd.append("file", f);
        fd.append("Id", $scope.parcela.id);


        $http.post("../../../Contrato/ImportCSVPedido", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            Metronic.unblockUI();

            if (data.Situacao == "OK") {
                $scope.somaaposcarga = true;
                $scope.LoadPedidos();


            } else {
                $scope.showMSGUpPedido = true;
                $scope.msgMSGUpPedido = importacaoArquivo.Situacao;
            };


        }).error(function (data) {
            Metronic.unblockUI();

            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.pedido = { "Valor": 0, "Descricao": "", "Pedido": "", "unidadenegocio_id": 0 };
    $scope.ListaContratoItemPedido = [];
    $scope.showMSGPedido = false;
    $scope.showMSGUpPedido = false;
    $scope.msgMSGPedido = "";
    $scope.msgMSGUpPedido = "";
    $scope.LoadBancos();
    $scope.LoadCodigoServicos();
    $scope.LoadOperacoes();
    $scope.somaaposcarga = false;

    $scope.LoadListaUnidades = function () {
        $http.post("../../CG/ObterUnidades")
            .success(function (data) {
                $scope.ListaUnidades = data;
            });
    };
    $scope.LoadListaUnidades();
}
var ControllerUpload = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.arquivo = "";

    $scope.someMsg = function () {
        $scope.showNOK = false;
        $scope.showOK = false;
    };

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        var descricao = document.getElementById('DescricaoArquivo').value;
        var f = document.getElementById('newPhotos').files[0];
        var fd = new FormData();
        fd.append("file", f);
        fd.append("id", $scope.item.item.id);
        fd.append("descricao", descricao);
        $http.post("../../../Contrato/Upload", fd, {
            withCredentials: true,
            headers: {'Content-Type': undefined },
            transformRequest: angular.identity
            }).success(function (data) {
                Metronic.unblockUI();

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
                        Metronic.unblockUI();

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

    $scope.carregaArquivos = function () {

    Metronic.blockUI({
        boxed: true
    });

    $http.post("../../../Contrato/Arquivos", { id: $scope.item.id })
    .success(function (data) {
        Metronic.unblockUI();

        $scope.ListaArquivos = data;
        $scope.showOK = true;

    }).error(function (data) {
        Metronic.unblockUI();
        $scope.showNOK = true;
        $scope.msgNOK = "Sistema indisponivel";
    });
    }

    $scope.Excluir = function (item) {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../../Contrato/RemoveArquivo", { id: item.id }).success(function (data) {
            Metronic.unblockUI();

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
                Metronic.unblockUI();

                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
    });


    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.carregaArquivos();
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

var ControllerMunicipios = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.ListaMunicipios = item.item;
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
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    Metronic.unblockUI();
};