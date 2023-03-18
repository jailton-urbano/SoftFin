setTimeout(
    function () {
        $("#DivTotal").fadeIn("slow");
    }, 1000);

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
    };
});

app.filter('entradasaida', function () {
    return function (input) {
        return input == "E" ? 'Entrada' : 'Saída';
    };
});

app.filter('status', function () {
    return function (input) {
        switch (input) {
            case 1:
                return '1 - RPS Emitido';
                
            case 2:
                return '2 - NFS-e Gerada';
                
            case 3:
                return '3 - NFS-e cancelada sem confirmação';
                
            case 4:
                return '4 - NFS-e cancelada com confirmação';
                
            case 5:
                return '5 - NFS-e baixada como perda';
                
            case 6:
                return '6 - NFS-e Avulsa';
                
            default:
                return input;
                
        }
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../NF/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        Metronic.blockUI({
            boxed: true
        });

        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.ModoConsulta = false;
        $scope.showGrid = true;
        $scope.showCalcular = false;
        $scope.showSalvar = true;
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaIndicadorPessoa = [{ Value: 1, Text: "CPF" }, { Value: 2, Text: "CNPJ" }];

        $scope.ListaStatus = [
            { Value: 1, Text: "1 - RPS Emitido" },
            { Value: 2, Text: "2 - NFS-e Gerada" },
            { Value: 3, Text: "3 - NFS-e cancelada sem confirmação" },
            { Value: 4, Text: "4 - NFS-e cancelada com confirmação" },
            { Value: 5, Text: "5 - NFS-e baixada como perda" },
            { Value: 6, Text: "6 - NFS-e Avulsa" },
            { Value: 0, Text: "Todos" }

        ];

        $scope.ListaRespRetencao = [
            { Value: "1", Text: "1 – Tomador" },
            { Value: "2", Text: "2 – Intermediário" },
            { Value: "3", Text: "3 – Prestador" }
        ];

        $scope.ListaLocalPrestServ = [
            { Value: "1", Text: "1 - No Município Sem Retenção " },
            { Value: "2", Text: "2 - No Município Com Retenção" },
            { Value: "3", Text: "3 - Fora do Município Sem Retenção " },
            { Value: "4", Text: "4 - Fora do Município Com Retenção  " },
            { Value: "5", Text: "5 - Fora do Município Com pagamento no local" }
        ];

        

        $scope.ListaOV = [];
        $scope.BuscaOV = function () {
            $scope.ModoConsulta = false;
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../NF/ObterOrdemVendaAberto")
                .success(function (data) {
                    $scope.ListaOV2 = data;

                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: false,
                        keyboard: true,
                        templateUrl: '../../page/OV/consultaNF.html',
                        controller: ControllerOV,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: $scope.ListaOV2 }); // pass name to Dialog
                    };

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function (value) {
                        $scope.GerarNotaFaturamento(value);
                    }, function () {
                        $scope.Pesquisa();
                    });

                })
                .error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";
                    Metronic.unblockUI();
                });
        };
        $scope.Detalhe = function (item) {
            $scope.ObterPorId(item.id);

            $scope.showGrid = false;
            $scope.showSalvar = true;
            $scope.showCalcular = true;
            $scope.ModoConsulta = true;
            $scope.showSalvar = false;
        };

        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);

            $scope.showGrid = false;
            $scope.showCalcular = true;
            $scope.showSalvar = true;
        };
        $scope.ObterPorId = function (id) {
            $scope.ShowManut = true;
            $http.get("../../NF/ObterPorId/" + id)
                .success(function (data) {
                    $scope.filtro = data.OV;
                    $scope.nf = data.NF;
                    $scope.nf.NotaFiscalPessoaTomador = data.NotaFiscalPessoaTomador;
                }).error(function (data) {
                    $scope.MsgNOK = "Sistema indisponivel";
                });
        }
        $scope.AlterarVencimento = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalVencimento.html',
                controller: ControllerVencimento,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });
        };

        $scope.CalculaNota = function () {
            $scope.showNOK = false;
            Metronic.blockUI({
                boxed: true
            });

            $scope.nf_id = 0;
            if ($scope.nf !== undefined)
                if ($scope.nf.id !== undefined)
                    $scope.nf_id = $scope.nf.id;

            $http.post("../../NF/CalculaNotaTela",
                {
                    codigoServico: $scope.filtro.codigoServico,
                    data: $scope.filtro.data,
                    banco_id: $scope.filtro.banco_id,
                    operacao_id: $scope.filtro.operacao_id,
                    valor: $scope.filtro.valor,
                    unidadeNegocio_ID: $scope.filtro.unidadeNegocio_ID,
                    ovid: $scope.filtro.id,
                    pessoastr: $scope.filtro.pessoanome,
                    nota_id: $scope.nf_id,
                    valorDeducoes: $scope.filtro.valorDeducoes
                })
                .success(function (data) {
                    Metronic.unblockUI();
                    if (data.CDStatus == "OK") {
                        $scope.nf = data.obj;
                        $scope.ov = data.ov;
                        $scope.showCalcular = true;
                    }
                    else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;


                        $timeout(
                            function () {
                                $scope.showNOK = false;
                                $scope.showOK = false;

                            }, 29000);
                    }
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Verifique as informações do calculo";
                });
        };

        $scope.LoadListaOV = function () {
            $scope.MsgPesquisa = "Aguarde...";
            $scope.ListaOV = [];
            $http.post("../../NF/ObterNF", $scope.filtro2)
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaOV = data;


                    if (data.lenght == undefined)
                        $scope.MsgPesquisa = "Não foram encontrados ordem de vendas pendentes";
                });
        }

        $scope.GerarNota = function () {
            $scope.filtro = {};
            $scope.filtro.situacaoPrefeitura = "Nota Avulsa";
            $scope.filtro.situacaoPrefeitura_id = 6;
            $scope.filtro.id = 0;
            $scope.showGrid = false;
            $scope.showCalcular = false;
            $scope.showSalvar = true;
        };
        $scope.GerarNotaFaturamento = function (item) {
            $scope.filtro = item;
            $scope.filtro.situacaoPrefeitura = "Nota Fiscal de Serviço Não Enviada";
            $scope.filtro.situacaoPrefeitura_id = 1;
            $scope.filtro.dataVencimentoNfse = $scope.filtro.DataVencimentoOriginal;

            $scope.showGrid = false;
            $scope.showCalcular = false;
            $scope.showSalvar = true;
        };


        $scope.Voltar = function (item) {
            $scope.showGrid = true;
            $scope.showOK = false;
            $scope.showNOK = false;
            $scope.ModoConsulta = false;
            $scope.showSalvar = true;
            $scope.Pesquisa();
        };
        $scope.Pesquisa = function (msg) {
            $scope.LoadListaOV();
        };


        $scope.Boleto = function (item) {
            window.open("../Recebimento/SeletorBanco/" + item.id, "_blank");
        }

        //$scope.Pesquisa();
        $scope.Salvar = function () {
            Metronic.blockUI({
                boxed: true
            });
            $scope.showNOK = false;
            $scope.showOK = false;
            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;
            $scope.nf.dataEmissaoNfse = $scope.filtro.data;
            $scope.nf.DataVencimentoOriginal = $scope.filtro.DataVencimentoOriginal;
            $scope.nf.DataVencimento = $scope.filtro.DataVencimentoOriginal;
            $scope.nf.codigoVerificacao = $scope.filtro.codigoVerificacao;

            if ($scope.nf.situacaoPrefeitura_id === 0)
                $scope.nf.situacaoPrefeitura_id = $scope.filtro.situacaoPrefeitura_id;

            $scope.nf.itemProdutoServico_ID = $scope.filtro.itemProdutoServico_ID;
            $scope.nf.numeroNfse = $scope.filtro.numeroNfse
            $scope.nf.codigoVerificacao = $scope.filtro.codigoVerificacao;
            $scope.nf.codigoServico = $scope.filtro.codigoServico;
            $scope.nf.operacao_id = $scope.filtro.operacao_id;
            $scope.nf.unidadeNegocio_ID = $scope.filtro.unidadeNegocio_ID;
            $scope.nf.tabelaPreco_ID = $scope.filtro.tabelaPreco_ID;
            $scope.nf.operacao_id = $scope.filtro.operacao_id;
            $scope.nf.banco_id = $scope.filtro.banco_id;

           
            var ovtemp = $scope.filtro;
            
            
            var postdata = { notafiscal: $scope.nf, ov: ovtemp };
            $http.post("../../NF/Salvar", postdata)
                .success(function (data) {
                    Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    
                    $scope.showOK = true;
                    $scope.showSalvar = false;
                    $scope.ErrosPrefeitura = data.ErrosPrefeitura;
                    $scope.AlertasPrefeitura = data.AlertasPrefeitura;
                    $scope.msgOK = data.DSMessage;
                    $scope.msgSalvar = "Salvar";
                } else {
                   
                    $scope.showNOK = true;
                    $scope.Erros = data.Erros;
                    $scope.msgNOK = data.DSMessage;


                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                            $scope.msgSalvar = "Salvar";
                            $scope.btnSalvar = true;

                        }, 29000);
                }

            })
                .error(function (data) {
                   Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
                $scope.msgSalvar = "Salvar";
                $scope.btnSalvar = true;
            });
        }

        $scope.Cancelamento = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalCancelamento.html',
                controller: ControllerCancelamento,
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

        $scope.Baixa = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalBaixa.html',
                controller: ControllerBaixa,
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


        $scope.Envio = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalEnvio.html',
                controller: ControllerEnvio,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });
        }


        $scope.Consulta = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalConsulta.html',
                controller: ControllerConsulta,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });
        }
        $scope.Historico = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalHistorico.html',
                controller: ControllerHistorico,
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
        $scope.LoadBancos = function () {
            $http.post("../../NF/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadCodigoServicos = function () {

            $http.post("../../NF/ObterCodigoServicos")
                .success(function (data) {
                    $scope.ListaCodigoServicos = data;
                });
        };
        $scope.LoadOperacoes = function () {

            $http.post("../../NF/ObterOperacoes")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaOperacoes = data;
                });
        };
        $scope.LoadListaPessoas = function () {
            $http.post("../../NF/ListaPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        }

        $scope.LoadUnidadeNegocio = function () {

            $http.post("../../NF/ObterUnidadeNegocios")
                .success(function (data) {
                    $scope.ListaUnidadeNegocios = data;
                });
        };


        $scope.LoadItemProdutoServicos = function () {

            $http.post("../../NF/ObterItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaProdutoServicos = data;
                });
        };

        $scope.LoadTabelaPrecoItemProdutoServicos = function () {

            $http.post("../../NF/ObterTabelaPrecoItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaTabelaPrecos = data;
                });
        };

        $scope.uploadLacto = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalUploadLacto.html',
                controller: ControllerUploadLacto,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: 0 }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                if (data.CDStatus == "OK")
                {
                    $scope.showGrid = false;
                    $scope.showSalvar = true;
                    $scope.showCalcular = true;
                    $scope.ModoConsulta = false;
                    $scope.showSalvar = true;
                    $scope.nf = data.nf;
                    $scope.ov = data.ov;
                    $scope.filtro = data.filtro;
                }
            }, function () {

            });
        };

        $scope.LoadItemProdutoServicos();
        $scope.LoadTabelaPrecoItemProdutoServicos();
        $scope.LoadListaPessoas();
        $scope.LoadUnidadeNegocio();
        $scope.LoadBancos();
        $scope.LoadCodigoServicos();
        $scope.LoadOperacoes();
        $scope.LoadListaPessoas();
    }
]);

var ControllerCancelamento = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../NF/Cancelamento", $scope.item)
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

                }, 29000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerBaixa = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../NF/BaixaPerda", $scope.item)
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

                }, 29000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerEnvio = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../NF/NFXMLEnvio", $scope.item)
        .success(function (data) {
            $scope.Erros = data.Erros;
            $scope.Alertas = data.Alertas;
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;

            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 29000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerConsulta = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../NF/NFXMLConsulta", $scope.item)
        .success(function (data) {

            $scope.Erros = data.Erros;
            $scope.Alertas = data.Alertas;
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;

            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 29000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerHistorico = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $http.post("../../NF/Historico", $scope.item)
    .success(function (data) {
        if (data.CDStatus == "OK") {
            $scope.objs = data.objs;
        } else {
            $scope.showNOK = true;
            $scope.msgNOK = data.DSMessage;
        }
        $timeout(
            function () {
                $scope.showNOK = false;
                $scope.showOK = false;

            }, 29000);
    }).error(function (data) {
        $scope.showNOK = true;
        $scope.msgNOK = "Sistema indisponivel";
    });


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerVencimento = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.vencimento = "";
    $scope.ok = function () {
        $http.post("../../NF/AlterarVencimento", { id: $scope.item.id, estab: $scope.item.estabelecimento_id, vencimento: $scope.vencimento })
        .success(function (data) {


            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;

            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 29000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }



    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerUploadLacto = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.arquivo = "";
    $scope.ok = function () {

        var f = document.getElementById('newPhotos').files[0];
        var fd = new FormData();
        fd.append("arquivo", f);

        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../../NF/ObterNFPorXML", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            Metronic.unblockUI();

            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            };

            

            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 29000);


        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerOV = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.ListaOV = item.item;
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