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

app.directive('datetimey', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            var a = element.datetimepicker({
                dateFormat: 'dd/MM/yyyy hh:mm',
                language: 'ptbr',
                pickTime: false,
                startDate: '01-11-2013',      // set a minimum date
                endDate: '01-11-2030',
                autoclose: true,
                minView: 2

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

app.filter('entradasaida', function () {
    return function (input) {
        return input == "E" ? 'Entrada' : 'Saída';
    }
})


app.filter('status', function () {
    return function (input) {
        if (input == "1")
            return "Nota não enviada ao Sefaz(Pode ser alterada)";
        if (input == "2")
            return "Nota enviada ao Sefaz";
        if (input == "3")
            return "Nota cancelada pendete de envio ao Sefaz";
        if (input == "4")
            return "Nota Cancelada e confirmada no Sefaz";
        if (input == "5")
            return "Baixa por Perda";
    }
})

app.filter('filtroindPag', function () {
    return function (input) {
        if (input == 0)
            return "Pagamento à Vista";
        if (input == 1)
            return "Pagamento à Prazo";
        return "-";
    }
})

app.filter('filtrotPag', function () {
    return function (input) {
        if (input == "01")
            return "Dinheiro";
        if (input == "02")
            return "Cheque";
        if (input == "03")
            return "Cartão de Crédito";
        if (input == "04")
            return "Cartão de Débito";
        if (input == "05")
            return "Crédito Loja";
        if (input == "10")
            return "Vale Alimentação";
        if (input == "11")
            return "Vale Refeição";
        if (input == "12")
            return "Vale Presente";
        if (input == "13")
            return "Vale Combustível";
        if (input == "14")
            return "Duplicata Mercantil";
        if (input == "13")
            return "Boleto Bancário";
        if (input == "90")
            return "Sem pagamento";
        if (input == "99")
            return "Outros";
        return "-";
    }
})

app.filter('filtrotpIntegra', function () {
    return function (input) {
        if (input == 1)
            return "Pagamento integrado com o sistema de automação da empresa(Ex.: equipamento TEF, Comércio Eletrônico)";
        if (input == 2)
            return "Pagamento não integrado com o sistema de automação da empresa(Ex.: equipamento POS)";
        return "-";
    }
})

app.filter('filtrotBand', function () {
    return function (input) {
        if (input == "01")
            return "Visa";
        if (input == "02")
            return "Mastercard";
        if (input == "03")
            return "American Express";
        if (input == "04")
            return "Sorocred";
        if (input == "05")
            return "Diners Club";
        if (input == "06")
            return "Elo";
        if (input == "07")
            return "Hipercard";
        if (input == "08")
            return "Aura";
        if (input == "09")
            return "Cabal";
        if (input == "10")
            return "Outros";
        return "-";
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../NFe/AcessoEdicao")
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
        $scope.reverseItem = false;
        $scope.reverseReboques = false;
        $scope.reverseVolumes = false;
        $scope.reverseDuplicatas = false;
        $scope.reversenfereferenciadas = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.ModoConsulta = false;
        $scope.showGrid = true;
        $scope.showCalcular = false;
        $scope.showSalvar = true;
        $scope.listaDuplicatas = [];
        $scope.listaNFReferenciada = [];
        $scope.listaReboques = [];
        $scope.listaVolumes = [];
        $scope.mensagemPadrao = 0;


        $scope.BuscaOV = function () {
            $scope.ModoConsulta = false;
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../NFe/ObterOrdemVendaAberto")
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


        $scope.GerarNotaFaturamento = function (item) {
            debugger
            $scope.ObterNFporID(0, item.id, false);

            $scope.showGrid = false;
            $scope.showCalcular = false;
            $scope.showSalvar = true;
        };


        $scope.IncluirDuplicata = function () {

            Metronic.blockUI({
                boxed: true
            });

            var duplicata = {
                id: 0,
                notaFiscal_id: 0,
                numero: '',
                vencto: '',
                valor: 0
            };

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalDuplicata.html',
                controller: ControllerDuplicata,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: duplicata }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.listaDuplicatas.push(value);
            }, function () {
            });


        }
        $scope.ExcluirDuplicata = function (id) {
            $scope.listaDuplicatas.splice(id, 1);
        }

        $scope.IncluirVolume = function () {

            Metronic.blockUI({
                boxed: true
            });

            var volume = {
                id: 0,
                qtde: 0,
                notaFiscal_id: 0,
                numero: '',
                vencto: '',
                valor: 0
            };

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalVolume.html',
                controller: ControllerVolume,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: volume }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {

                $scope.listaVolumes.push(value);
            }, function () {
            });


        }
        $scope.ExcluirVolume = function (id) {
            $scope.listaVolumes.splice(id, 1);
        }


        $scope.IncluirReboque = function () {

            Metronic.blockUI({
                boxed: true
            });

            var reboque = {
                id: 0,
                notaFiscal_id: 0,
                numero: '',
                vencto: '',
                valor: 0
            };

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalReboque.html',
                controller: ControllerReboque,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: reboque }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.listaReboques.push(value);
            }, function () {
            });


        }
        $scope.ExcluirReboque = function (id) {
            $scope.listaReboques.splice(id, 1);
        }


        $scope.ListaModalidadeFrete = [
            { "Value": "0", "Text": "0 = Por conta do emitente" },
            { "Value": "1", "Text": "1 = Por conta do destinatário/remetente;" },
            { "Value": "2", "Text": "2 = Por conta de terceiros" }
        ];


        $scope.ListaTipoNFe = [
            { "Value": "1", "Text": "1 = NFe Normal" },
            { "Value": "2", "Text": "2 = NFe Complementar" },
            { "Value": "3", "Text": "3 = NFe de Ajuste" },
            { "Value": "4", "Text": "4 = Devolução/Retorno" }
        ];

        $scope.Estados = [{
            "ID": "1",
            "Sigla": "AC",
            "Nome": "Acre"
        },
         {
             "ID": "2",
             "Sigla": "AL",
             "Nome": "Alagoas"
         },
         {
             "ID": "3",
             "Sigla": "AM",
             "Nome": "Amazonas"
         },
         {
             "ID": "4",
             "Sigla": "AP",
             "Nome": "Amapá"
         },
         {
             "ID": "5",
             "Sigla": "BA",
             "Nome": "Bahia"
         },
         {
             "ID": "6",
             "Sigla": "CE",
             "Nome": "Ceará"
         },
         {
             "ID": "7",
             "Sigla": "DF",
             "Nome": "Distrito Federal"
         },
         {
             "ID": "8",
             "Sigla": "ES",
             "Nome": "Espírito Santo"
         },
         {
             "ID": "9",
             "Sigla": "GO",
             "Nome": "Goiás"
         },
         {
             "ID": "10",
             "Sigla": "MA",
             "Nome": "Maranhão"
         },
         {
             "ID": "11",
             "Sigla": "MG",
             "Nome": "Minas Gerais"
         },
         {
             "ID": "12",
             "Sigla": "MS",
             "Nome": "Mato Grosso do Sul"
         },
         {
             "ID": "13",
             "Sigla": "MT",
             "Nome": "Mato Grosso"
         },
         {
             "ID": "14",
             "Sigla": "PA",
             "Nome": "Pará"
         },
         {
             "ID": "15",
             "Sigla": "PB",
             "Nome": "Paraíba"
         },
         {
             "ID": "16",
             "Sigla": "PE",
             "Nome": "Pernambuco"
         },
         {
             "ID": "17",
             "Sigla": "PI",
             "Nome": "Piauí"
         },
         {
             "ID": "18",
             "Sigla": "PR",
             "Nome": "Paraná"
         },
         {
             "ID": "19",
             "Sigla": "RJ",
             "Nome": "Rio de Janeiro"
         },
         {
             "ID": "20",
             "Sigla": "RN",
             "Nome": "Rio Grande do Norte"
         },
         {
             "ID": "21",
             "Sigla": "RO",
             "Nome": "Rondônia"
         },
         {
             "ID": "22",
             "Sigla": "RR",
             "Nome": "Roraima"
         },
         {
             "ID": "23",
             "Sigla": "RS",
             "Nome": "Rio Grande do Sul"
         },
         {
             "ID": "24",
             "Sigla": "SC",
             "Nome": "Santa Catarina"
         },
         {
             "ID": "25",
             "Sigla": "SE",
             "Nome": "Sergipe"
         },
         {
             "ID": "26",
             "Sigla": "SP",
             "Nome": "São Paulo"
         },
         {
             "ID": "27",
             "Sigla": "TO",
             "Nome": "Tocantins"
         }];

        $scope.RecalcularNota = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../NFe/CalculaItem", { notaFiscalNFEItem: null, notaFiscalNFEItems: $scope.nfeItems, notaFiscalNFE: $scope.nfe })
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.nfeItems = data.NotaFiscalNFEItems;
                    $scope.nfe = data.NotaFiscalNFE;
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


        $scope.IncluirNFReferenciada = function () {

            Metronic.blockUI({
                boxed: true
            });

            var nfReferenciada = {
                id: 0,
                notaFiscal_id: 0,
                NFe: '',
                CTe: '',
                nfserie: '',
                nfnumero: '',
                nfmodelo: '',
                nfuf: '',
                nfanoMesEmissao: '',
                nfcnpj: '',
                nfprodserie: '',
                nfprodnumero: '',
                nfprodmodelo: '',
                nfproduf: '',
                nfprodanoMesEmissao: '',
                nfprodcnpjCpf: '',
                nfprodIE: '',
                ECF: '',
                numeroCOO: '',
                modelo: ''
            };

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalNFReferenciada.html',
                controller: ControllerNFReferenciada,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: nfReferenciada }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.listaNFReferenciada.push(value);
            }, function () {
            });


        }
        $scope.ExcluirNFReferenciada = function (id) {
            $scope.listaNFReferenciada.splice(id, 1);
        }
        $scope.ObtemItemInicial = function () {

            $scope.ObterNFporID(0, 0, false);
        };

        $scope.ObterNFporID = function (value, ovid, copiar) {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../NFe/ObterNFPorId", { 'id': value, 'ovid': ovid,  'copiar': copiar })
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ov = data.ov;
                    $scope.nf = data.nf;
                    $scope.nfe = data.nfe;
                    $scope.transp = data.transp;
                    $scope.nfeItem = data.NotaFiscalNFEItem;
                    $scope.nfeItems = data.NotaFiscalNFEItems;
                    $scope.retirada = data.retirada;
                    $scope.entrega = data.entrega;
                    $scope.NotaFiscalPessoaTomador = data.NotaFiscalPessoaTomador;
                    $scope.listaVolumes = data.listaVolumes;
                    $scope.listaDuplicatas = data.listaDuplicatas;
                    $scope.listaReboques = data.listaReboques;
                    $scope.listaNFReferenciada = data.listaNFReferenciada;
                    $scope.listaFormaPagamento = data.listaFormaPagamento ;

            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
        }


        $scope.ObtemPessoa = function () {

            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../NFe/ObtemPessoa", { pessoanome: $scope.ov.pessoanome })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.NotaFiscalPessoaTomador = data;
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
        }
        $scope.ExcluirItem = function (id) {
            $scope.nfeItems.splice(id, 1);
            $scope.RecalcularNota();
        }
        $scope.ExcluirFP = function (id) {
            $scope.listaFormaPagamento.splice(id, 1);
        }

        $scope.NovoItem = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../NFe/ObterItem", { notaFiscalNFEItem: null, notaFiscalNFEItems: $scope.nfeItems })
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus == "OK") {
                        $scope.opts = {
                            size: 'lg',
                            animation: true,
                            backdrop: false,
                            backdropClick: false,
                            dialogFade: false,
                            keyboard: true,
                            templateUrl: 'modalItem.html',
                            controller: ControllerItem,
                            resolve: {} // empty storage
                        };

                        data.nfe = $scope.nfe;

                        $scope.opts.resolve.item = function () {
                            return angular.copy({ item: data }); // pass name to Dialog
                        };

                        var modalInstance = $modal.open($scope.opts);

                        modalInstance.result.then(function (value) {
                            $scope.nfeItems = value.NotaFiscalNFEItems;
                            $scope.nfe = value.NotaFiscalNFE;
                        }, function () {
                        });


                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
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

        $scope.NovoFP = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalFP.html',
                controller: ControllerFP,
                resolve: {} // empty storage
            };

            if (item != null) {
                $scope.opts.resolve.item = function () {
                    return angular.copy({ item: item });
                };
            }
            else {
                $scope.opts.resolve.item = function () {
                    return angular.copy({
                        item: {
                            "indPag": 0, "tPag": "01", "vPag": $scope.nfe.valor, "tpIntegra": null, "CNPJ": null, "tBand": null, "cAut": null, "vTroco": null
                        }
                    });
                };
            }


            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                Metronic.unblockUI();
                if (item == null) {
                    $scope.listaFormaPagamento.push(value);
                } else {
                    var index = $scope.listaFormaPagamento.indexOf(item);
                    $scope.listaFormaPagamento.push(value);
                }
            }, function () {
                Metronic.unblockUI();
            });
        };

        $scope.AlterarItem = function (value, index) {

            Metronic.blockUI({
                boxed: true
            });

            value.item = index;

            $http.post("../../NFe/ObterItem", { notaFiscalNFEItem: value, notaFiscalNFEItems: $scope.nfeItems })
            .success(function (data) {
                Metronic.unblockUI();

                if (data.CDStatus == "OK") {
                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: false,
                        keyboard: true,
                        templateUrl: 'modalItem.html',
                        controller: ControllerItem,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: data }); // pass name to Dialog
                    }

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function (value) {
                        $scope.nfeItems = value;
                    }, function () {
                    });


                } else {
                    $scope.showNOK = true;
                    $scope.Erros = data.Erros;
                    $scope.msgNOK = data.DSMessage;
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
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ordenarDuplicatas = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ordenarItem = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverseItem = !$scope.reverseItem;
        };
        $scope.ordenarRefereciada = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reversenfereferenciadas = !$scope.reversenfereferenciadas;
        };
        $scope.ordenarVolumes = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverseVolumes = !$scope.reverseVolumes;
        };
        $scope.ordenarReboques = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverseReboques = !$scope.reverseReboques;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaIndicadorPessoa = [{ Value: 1, Text: "CPF" }, { Value: 2, Text: "CNPJ" }];

        $scope.ListafaturaFormaPgto = [{ Value: 0, Text: "Pagamento à vista" }, { Value: 2, Text: "Pagamento a prazo" }, { Value: 3, Text: "Outros" }];

        $scope.ListaIndicadorPresencaComprador = [
            { Value: 0, Text: "0 = Não se aplica" },
            { Value: 1, Text: "1 = Operação presencial" },
            { Value: 2, Text: "2 = Operação não presencial, pela internet" },
            { Value: 3, Text: "3 = Operação não presencial, Teleatendimento" },
            { Value: 4, Text: "4 = NFC-e em operação com entrega em domicilio" },
            { Value: 9, Text: "2 = Operação não presencial, outros" }

        ];
        $scope.ListaOV = [];
        $scope.Copiar = function (item) {
            $scope.ObterNFporID(item.id, 0, true);
            $scope.showGrid = false;
            $scope.showSalvar = true;
            $scope.showCalcular = true;
            $scope.ModoConsulta = false;
            $scope.showSalvar = false;
        }
        $scope.Detalhe = function (item) {
            $scope.ObterNFporID(item.id, 0,false);
            $scope.showGrid = false;
            $scope.showSalvar = true;
            $scope.showCalcular = true;
            $scope.ModoConsulta = true;
            $scope.showSalvar = false;
        }

        $scope.Alterar = function (item) {
            $scope.ObterNFporID(item.id, 0, false);
            $scope.showGrid = false;
            $scope.showSalvar = true;
            $scope.showCalcular = true;
            $scope.ModoConsulta = false;
            $scope.showSalvar = false;
        }

        $scope.AlterarVencto = function (item) {
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
        }
        $scope.CalculaNota = function () {
            $http.post("../../NFe/CalculaNotaTela",
                {
                    codigoServico: $scope.nfe.codigoservico,
                    data: $scope.nfe.data,
                    bancoid: $scope.nfe.bancoid,
                    operacaoid: $scope.nfe.operacaoid,
                    valor: $scope.nfe.valor,
                    unidadeNegocioid: $scope.nfe.unidadeNegocioid,
                    ovid: 0,
                    pessoastr: $scope.nfe.pessoastr
                })
                .success(function (data) {
                    if (data.CDStatus == "OK") {
                        $scope.nf = data.obj;
                        $scope.ov = data.ov;
                        $scope.showCalcular = true;
                        $scope.nf.numeroNfse = $scope.nfe.numeroNfse;
                        $scope.nf.dataEmissaoNfse = $scope.nfe.data;
                        $scope.nf.codigoVerificacao = $scope.nfe.codigoVerificacao;

                    }
                    else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;


                        $timeout(
                            function () {
                                $scope.showNOK = false;
                                $scope.showOK = false;

                            }, 8000);
                    }
                });
        };
        $scope.LoadListaOV = function () {
            $scope.MsgPesquisa = "Aguarde...";
            $scope.ListaOV = [];
            $http.post("../../NFe/ObterNF", $scope.nfe)
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaOV = data;


                    if (data.lenght == undefined)
                        $scope.MsgPesquisa = "Não foram encontrados ordem de vendas pendentes";
                });
        };
        $scope.GerarNota = function (item) {
            $scope.ObtemItemInicial();
            $scope.nfe = item;
            $scope.showGrid = false;
            $scope.showCalcular = false;
            $scope.showSalvar = true;
        }
        $scope.Voltar = function (item) {
            
            $scope.showGrid = true;
            $scope.showOK = false;
            $scope.ModoConsulta = false;
            $scope.showSalvar = true;
            $scope.Pesquisa();
        }
        $scope.Pesquisa = function (msg) {
            $scope.LoadListaOV();
        };
        $scope.Boleto = function (item) {
            window.open("../Recebimento/SeletorBanco/" + item.id, "_blank");
        };
        $scope.Salvar = function () {
            Metronic.blockUI({
                boxed: true
            });

            $scope.showNOK = false;
            $scope.showOK = false;
            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;



            $scope.ov.valor = $scope.nfe.valor;

            var nfenvio = $scope.nf;
            nfenvio.NotaFiscalNFE = $scope.nfe;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEItems = $scope.nfeItems;
            nfenvio.NotaFiscalNFE.NotaFiscalNFETransportadora = $scope.transp;
            nfenvio.NotaFiscalNFE.NotaFiscalNFERetirada = $scope.retirada;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEEntrega = $scope.entrega;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEReferenciadas = $scope.listaNFReferenciada;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEDuplicatas = $scope.listaDuplicatas;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEVolume = $scope.listaVolumes;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEReboques = $scope.listaReboques;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEReboques = $scope.listaReboques;
            nfenvio.NotaFiscalNFE.NotaFiscalNFEFormaPagamentos = $scope.listaFormaPagamento;
            nfenvio.NotaFiscalPessoaTomador = $scope.NotaFiscalPessoaTomador


            var postdata = { notafiscal: nfenvio, ov: $scope.ov };
            $http.post("../../NFe/Salvar", postdata)
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
                    $scope.msgSalvar = "Salvar";

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                            $scope.msgSalvar = "Salvar";
                            $scope.btnSalvar = true;

                        }, 18000);
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
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
            });
        }

        $scope.CartaCorrecao = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalCartaCorrecao.html',
                controller: ControllerCartaCorrecao,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
            });
        };
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
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
            });
        };
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
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });
        }

        $scope.GerarXML = function (item) {
            $http.post("../../NFe/GerarXML", item)
                .success(function (data) {
                }).error(function (data) {
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });
        };

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
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Pesquisa();
            }, function () {
                $scope.Pesquisa();
            });
        };
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

        $scope.Status = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalStatus.html',
                controller: ControllerStatus,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: "" }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {

            }, function () {
            });
        }

        $scope.LoadBancos = function () {
            $http.post("../../NFe/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadCodigoServicos = function () {

            $http.post("../../NFe/ObterCodigoServicos")
                .success(function (data) {
                    $scope.ListaCodigoServicos = data;
                });
        };


        $scope.LoadCFOP = function () {

            $http.post("../../NFe/ObterCFOP")
                .success(function (data) {
                    $scope.ListaCFOP = data;
                });
        };

        $scope.LoadListaPessoas = function () {
            $http.post("../../NFe/ListaPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        }
        $scope.LoadUnidadeNegocio = function () {

            $http.post("../../NFe/ObterUnidadeNegocios")
                .success(function (data) {
                    $scope.ListaUnidadeNegocios = data;
                });
        };
        $scope.LoadItemProdutoServicos = function () {

            $http.post("../../NFe/ObterItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaProdutoServicos = data;
                });
        };
        $scope.LoadTabelaPrecoItemProdutoServicos = function () {

            $http.post("../../NFe/ObterTabelaPrecoItemProdutoServicos")
                .success(function (data) {
                    $scope.ListaTabelaPrecos = data;

                });
        };
        $scope.ObterTodosNFeInformacaoDropDown = function () {

            $http.post("../../NFe/ObterTodosNFeInformacaoDropDown")
                .success(function (data) {
                    $scope.ListaMensagemPadrao = data;
                });
        };
        $scope.PreenchePadrao = function () {

            $http.post("../../NFe/ObterPorIdNFeInformacao", { id: $scope.mensagemPadrao })
                .success(function (data) {
                    $scope.nfe.informacaoComplementar = data.informacaoComplementar;
                    $scope.nfe.informacaoComplementarFisco = data.informacaoComplementarFisco;
                });
        };
        $scope.ConsultaPadrao = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: '../../page/NFeInformacao/consulta.html',
                controller: ControllerInf,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: 0 }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodosNFeInformacaoDropDown();
            }, function () {
                $scope.ObterTodosNFeInformacaoDropDown();
            });
        }

        $scope.BuscaPessoa = function (tipo) {

            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Pessoa/ListaPessoas")
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

                });
        }

        $scope.BuscaMunicipios = function (tipo) {

            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../NFe/ListaMunicipios")
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

                        if (tipo == "transp") {
                            $scope.transp.codigoMunicipioOcorrencia = value.codigoIBGE;
                        }
                        if (tipo == "retirada") {
                            $scope.retirada.cidade = value.DESC_MUNICIPIO;
                            $scope.retirada.codMunicipio = value.codigoIBGE;
                        }
                        if (tipo == "entrega") {
                            $scope.entrega.cidade = value.DESC_MUNICIPIO;
                            $scope.entrega.codMunicipio = value.codigoIBGE;
                        }


                    });

                });
        }

        $scope.ObterTodosNFeInformacaoDropDown();
        $scope.LoadItemProdutoServicos();
        $scope.LoadTabelaPrecoItemProdutoServicos();
        $scope.LoadListaPessoas();
        $scope.LoadUnidadeNegocio();
        $scope.LoadBancos();
        $scope.LoadCodigoServicos();
        $scope.LoadCFOP();
        $scope.LoadListaPessoas();
        $scope.Pesquisa();

    }
]);


var ControllerItem = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.entidade = item.item.NotaFiscalNFEItem;
    $scope.entidades = item.item.NotaFiscalNFEItems;
    $scope.nfe = item.item.nfe;

    $scope.entidade.valorUnitario = 0;


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../NFe/CalculaItem", { notaFiscalNFEItem: $scope.entidade, notaFiscalNFEItems: $scope.entidades, notaFiscalNFE: $scope.nfe })
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

    $scope.SomaTudo = function () {
        $scope.entidade.valor = 0;
        $scope.entidade.valor = ($scope.entidade.quantidade * $scope.entidade.valorUnitario) - $scope.entidade.desconto
    }

    $scope.CarregaPadroes = function () {
        angular.forEach($scope.objsProdutos, function (value) {
            if (value.Value == $scope.entidade.idProduto) {
                $scope.entidade.valor = value.Price;
                $scope.entidade.valorUnitario = value.Price;
                //$scope.entidade.unidadeMedida = value.Unit;
            }
        });

        $scope.SomaTudo();
    }

    $scope.ObterTabelas = function () {
        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;

        $http.post('../../Estoque/ListaTabelas')
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.ListaTabelas = data.objs;
                $scope.ObterProdutos();
            } else {
                $scope.showSalvar = false;
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            }
            Metronic.unblockUI();
        })
        .error(function (data) {
            $scope.showSalvar = false;
            $scope.showNOK = true;
            $scope.msgNOK = 'Sistema Indisponível';
        });

        Metronic.blockUI({
            boxed: true
        });

    }


    $scope.LoadOperacoes = function () {

        $http.post("../../NFe/ObterOperacoes")
            .success(function (data) {
                Metronic.unblockUI();
                $scope.ListaOperacoes = data;
            });
    };
    $scope.LoadOperacoes();

    $scope.ObterProdutos = function () {

        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;

        if ($scope.entidade.TabelaPrecoItemProdutoServico_id == null) {
            $scope.objsProdutos = [];
            return;
        }

        $http.post('../../NFe/ListaProdutos', { idTabela: $scope.entidade.TabelaPrecoItemProdutoServico_id })
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.objsProdutos = data.objs;
            } else {
                $scope.objsProdutos = [];
            }
            Metronic.unblockUI();
        })
        .error(function (data) {
            $scope.objsProdutos = [];
        });

        Metronic.blockUI({
            boxed: true
        });
    }

    $scope.ObterTabelas();




    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerDuplicata = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.duplicata = item.item;



    $scope.ok = function () {
        $modalInstance.close($scope.duplicata);
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    Metronic.unblockUI();

}
var ControllerNFReferenciada = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.nfReferenciada = item.item;



    $scope.ok = function () {
        $modalInstance.close($scope.nfReferenciada);
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    Metronic.unblockUI();

}
var ControllerReboque = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.reboque = item.item;



    $scope.ok = function () {
        $modalInstance.close($scope.reboque);
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    Metronic.unblockUI();

}
var ControllerVolume = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.Volume = item.item;



    $scope.ok = function () {
        $modalInstance.close($scope.volume);
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    Metronic.unblockUI();

}
var ControllerCancelamento = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.motivo = "";

    $scope.ok = function () {
        $http.post("../../NFe/Cancelamento", { obj: $scope.item, motivo: $scope.motivo })
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
var ControllerCartaCorrecao = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.motivo = "";
    $scope.msgOK = "OK";
    $scope.showOK = true;
    $scope.enableOK = false;

    $scope.ok = function () {
        $scope.msgOK = "Aguarde...";
        $scope.enableOK = true;
        $http.post("../../NFe/CartaCorrecao", { obj: $scope.item, correcao: $scope.motivo })
        .success(function (data) {
            $scope.msgOK = "OK";
            $scope.showOK = false;

            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }

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
        $http.post("../../NFe/BaixaPerda", $scope.item)
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
var ControllerEnvio = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../NFe/NFXMLEnvio", $scope.item)
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
var ControllerConsulta = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../NFe/NFXMLConsulta", $scope.item)
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
var ControllerHistorico = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.showDetalhe = false;
    Metronic.blockUI({
        boxed: true
    });

    $http.post("../../NFe/Historico", $scope.item)
        .success(function (data) {
            Metronic.unblockUI();
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

                }, 8000);
        }).error(function (data) {
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.cancelDetalhe = function () {
        $scope.showDetalhe = false;
    };


    $scope.DetalharHistorico = function (value) {
        Metronic.blockUI({
            boxed: true
        });

        $scope.ListaErros = [];
        $scope.ListaAlertas = [];
        $scope.showDetalhe = true;
        $http.post("../../NFe/HistoricoDetalhe", "{id:" + value.id + "}")
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $scope.ListaErros = data.listaErros;
                    $scope.ListaAlertas = data.listaAlertas;
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
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



}
var ControllerVencimento = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;
    $scope.vencimento = "";
    $scope.ok = function () {
        $http.post("../../NFe/AlterarVencimento", { id: $scope.item.id, estab: $scope.item.estabelecimento_id, vencimento: $scope.vencimento })
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
    }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    Metronic.unblockUI();
}
var ControllerInf = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.Lista = [];
    $scope.reverse = false;
    $scope.showOK = false;
    $scope.showNOK = false;
    $scope.showDados = false;
    $scope.showGrid = true;

    $scope.order = function (keyname) {
        $scope.sortKey = keyname;
        $scope.reverse = !$scope.reverse;
    };

    Metronic.blockUI({
        boxed: true
    });
    $scope.select = function (value) {
        $modalInstance.close(value);
    }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.cancelEdit = function () {
        $scope.showDados = false;
        $scope.showGrid = true;
    };
    $scope.new = function () {
        $http.post("../../NFe/ObterPorIdNFeInformacao", { id: 0 })
        .success(function (data) {
            $scope.entidade = data;
            $scope.showDados = true;
            $scope.showGrid = false;
        });
    };
    $scope.edit = function (item) {
        $http.post("../../NFe/ObterPorIdNFeInformacao", { id: item.id })
        .success(function (data) {
            $scope.entidade = data;
            $scope.showDados = true;
            $scope.showGrid = false;
        });
    };

    $scope.save = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../NFe/SalvarNFeInformacao", { obj: $scope.entidade })
        .success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
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
        $scope.showDados = true;
        $scope.showGrid = false;
    };

    $scope.ObterTodosNFeInformacao = function () {

        $http.post("../../NFe/ObterTodosNFeInformacao")
            .success(function (data) {
                $scope.Lista = data;
            });
    };

    $scope.ObterTodosNFeInformacao();
    Metronic.unblockUI();
}
var ControllerStatus = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.showOK = false;
    $scope.showNOK = false;
    $http.get("../../NFe/Status")
    .success(function (data) {
        if (data.CDStatus == "OK") {
            $scope.showOK = true;
            $scope.msgOK = data.DSMessage;
        } else {
            $scope.showNOK = true;
            $scope.msgNOK = data.DSMessage;
            $scope.Erros = data.Erros;
        }

    }).error(function (data) {
        $scope.showNOK = true;
        $scope.msgNOK = "Sistema indisponivel";
    });


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };




}


var ControllerFP = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.entidade = item.item;

    $scope.ListaFP = [
        { Value: 0, Text: "Pagamento à Vista" },
        { Value: 1, Text: "Pagamento à Prazo" }
    ];

    $scope.ListaMP = [
        { Value: '01', Text: "Dinheiro" },
        { Value: '02', Text: "Cheque" },
        { Value: '03', Text: "Cartão de Crédito" },
        { Value: '04', Text: "Cartão de Débito" },
        { Value: '05', Text: "Crédito Loja" },
        { Value: '10', Text: "Vale Alimentação" },
        { Value: '11', Text: "Vale Refeição" },
        { Value: '12', Text: "Vale Presente" },
        { Value: '13', Text: "Vale Combustível" },
        { Value: '14', Text: "Duplicata Mercantil" },
        { Value: '15', Text: "Boleto Bancário" },
        { Value: '90', Text: "Sem pagamento" },
        { Value: '99', Text: "Outros" }
    ];

    $scope.ListaTI = [
        { Value: 1, Text: "Pagamento integrado com o sistema de automação da empresa (Ex.: equipamento TEF, Comércio Eletrônico);" },
        { Value: 2, Text: "Pagamento não integrado com o sistema de automação da empresa (Ex.: equipamento POS)" }
    ];

    $scope.ListaTB = [
        { Value: '01', Text: "Visa" },
        { Value: '02', Text: "Mastercard" },
        { Value: '03', Text: "American Express" },
        { Value: '04', Text: "Sorocred" },
        { Value: '05', Text: "Diners Club" },
        { Value: '06', Text: "Elo" },
        { Value: '07', Text: "Hipercard" },
        { Value: '08', Text: "Aura" },
        { Value: '09', Text: "Cabal" },
        { Value: '99', Text: "Outros" }
    ];


    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $modalInstance.close($scope.entidade);

    }
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