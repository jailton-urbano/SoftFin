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
        return input === "True" ? 'Sim' : 'Não';
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
	    $scope.showOK = false;
	    $scope.showNOK = false;
	    $scope.msgOK = "";
	    $scope.msgNOK = "";
	    $scope.GERACPAG = false;
        $scope.showGrid = true;
        $scope.keyname = "";
        $scope.reverse = true;

        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaSobra = [{ Value: "U", Text: "Última Parcela" }, { Value: "P", Text: "Primeira Parcela" }];
        $scope.ListaInstituicao = [{ Value: "Prefeitura", Text: "Prefeitura" }, { Value: "Sefaz", Text: "Sefaz" }];

        Metronic.blockUI({
		    boxed: true
        });

	    $scope.ordenar = function (keyname) {
		    $scope.sortKey = keyname;
		    $scope.reverse = !$scope.reverse;
        };

        $scope.LoadCFOP = function () {

            $http.post("../../NFe/ObterCFOP")
                .success(function (data) {
                    $scope.ListaCFOP = data;
                });
        };
        $scope.LoadCFOP();

        //ListaTipoNFe

        $scope.ListaTipoNFe = [
            { "Value": "1", "Text": "1 = NFe Normal" },
            { "Value": "2", "Text": "2 = NFe Complementar" },
            { "Value": "3", "Text": "3 = NFe de Ajuste" },
            { "Value": "4", "Text": "4 = Devolução/Retorno" }
        ];

        //ListaBancos

        $scope.LoadBancos = function () {
            $http.post("../../NFe/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadBancos();

        //ListaUnidadeNegocios

        $scope.LoadUnidadeNegocio = function () {

            $http.post("../../NFe/ObterUnidadeNegocios")
                .success(function (data) {
                    $scope.ListaUnidadeNegocios = data;
                });
        };
        $scope.LoadUnidadeNegocio();

        //ListaIndicadorPessoa
        $scope.ListaIndicadorPessoa = [{ Value: 1, Text: "CPF" }, { Value: 2, Text: "CNPJ" }];

        //ListaModalidadeFrete
        $scope.ListaModalidadeFrete = [
            { "Value": "0", "Text": "0 = Por conta do emitente" },
            { "Value": "1", "Text": "1 = Por conta do destinatário/remetente;" },
            { "Value": "2", "Text": "2 = Por conta de terceiros" }
        ];

        //Estados


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

        //ListafaturaFormaPgto
        $scope.ListafaturaFormaPgto = [{ Value: 0, Text: "Pagamento à vista" }, { Value: 2, Text: "Pagamento a prazo" }, { Value: 3, Text: "Outros" }];


        //ListaIndicadorPresencaComprador
        $scope.ListaIndicadorPresencaComprador = [
            { Value: 0, Text: "0 = Não se aplica" },
            { Value: 1, Text: "1 = Operação presencial" },
            { Value: 2, Text: "2 = Operação não presencial, pela internet" },
            { Value: 3, Text: "3 = Operação não presencial, Teleatendimento" },
            { Value: 4, Text: "4 = NFC-e em operação com entrega em domicilio" },
            { Value: 9, Text: "2 = Operação não presencial, outros" }];

	$scope.item = [];
	$scope.pessoa = [];
	$scope.nf = [];
	$scope.CPAG = [];
	$scope.CPAGITEMS = [];

	$scope.MsgError = function (msg) {
		$scope.msgNOK = msg;
		$scope.showNOK = true;
		$scope.showOK = false;
		//$scope.timeoutview = $timeout($scope.LimpaMensagens(), 50000);
	}

	$scope.MsgSucesso = function (msg) {
		$scope.msgOK = msg;
		$scope.showOK = true;
		$scope.showNOK = false;
		//$scope.timeoutview = $timeout($scope.LimpaMensagens(), 50000);
	}

	$scope.ConfiguraDatas = function () {
		$http.get("../../NFSeImportacao/obtemData")
        .success(function (data) {
            $scope.filtro = data;
            Metronic.unblockUI();
        }).error(function (data) {
            $scope.MsgError("Sistema indisponivel");
            $scope.ListaNotas = [];
            Metronic.unblockUI();
        });
	}

	$scope.ConfiguraDatas();

	$scope.Pesquisa = function (msg) {
		        
		$scope.ListaNotasFiscais();
	}

	$scope.ListaNotasFiscais = function () {
        Metronic.blockUI({
            boxed: true
        });
		$http.post("../../NFImportacao/ListaNotas")
                    
            .success(function (data) {
                Metronic.unblockUI();
				if (data.CDStatus != "NOK") {
					$scope.ListaNotas = data.NFSe;

				}
				else {
					$scope.MsgError("Erro ao carregar as notas");
					$scope.Alertas = data.Alertas;
					$scope.Erros = data.Erros;
					$scope.ListaNotas = [];

				}
            }).error(function (data) {
                Metronic.unblockUI();
				$scope.MsgError("Sistema indisponivel");
				$scope.ListaNotas = [];

			});
	}

        $scope.Pesquisa();

    $scope.Importa = function (item) {

        Metronic.blockUI({
            boxed: true
        });
            
        $scope.showGrid = false;

		$("#liPessoa").addClass("active");
		$("#liNf").removeClass("active");
		$("#liCPAG").removeClass("active");

		$("#tabPessoa").addClass("active in");
		$("#tabnf").removeClass("active");
		$("#tabCpag").removeClass("active");

		$("#liPessoa.a").attr("aria-expanded", "true");
		$("#tabnf.a").attr("aria-expanded", "false");
		$("#tabCpag.a").attr("aria-expanded", "false");

		$scope.SomaTotal();
		        

		angular.forEach($scope.frmpessoas.$error.required, function (field) {
		    field.$setDirty();
        });

        $http.post("../../NFImportacao/ObtemNota", { "chNFe" : item.chNFe })
        .success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus != "NOK") {
                $scope.ListaNotas = data.NFSe;
            }
            else {
                $scope.MsgError("Erro ao carregar as notas");
                $scope.Alertas = data.Alertas;
                $scope.Erros = data.Erros;
                $scope.ListaNotas = [];
            }
        }).error(function (data) {
            Metronic.unblockUI();
            $scope.MsgError("Sistema indisponivel");
            $scope.ListaNotas = [];
        });
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

	$scope.CarregaCEP = function (data) {
		        
		$scope.pessoa.endereco = data.logradouro;
		$scope.pessoa.bairro = data.bairro;
        $scope.pessoa.cidade = data.localidade;
		$scope.pessoa.uf = data.uf;
	};


	$scope.OK = function () {
		        
		$scope.msgSalvar = "Aguarde";
		$scope.btnSalvar = false;

		var metodo = "SALVAR";
		if ($scope.GERACPAG) {
		    if ($scope.frmDadosGerais.$valid) {
		                
		        $scope.CPAGAux = $scope.CPAG;
		        $scope.CPAGAux.CodigoVerificacao = $scope.nf.codigoVerificacao;

		        var postdata = { pessoa: $scope.pessoa, notafiscal: $scope.nf, cpag: $scope.CPAGAux, cpagItems: $scope.CPAGITEMS, cpagParcelas: $scope.CPAGPARCELAS };//, notafiscal: $scope.nf, documentoPagarMestre: $scope.CPAG, documentoPagarDetalhes: $scope.CPAGITEMS };
		                
		    }
		    else {
		        $scope.showNOK = true;
		        $scope.msgNOK = "Verifique os campos do lançamento no Contas a Pagar.";

		                


		        $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;

                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 8000);
                return
		    }

		    metodo = "SalvarComCPAG";

		}
		else {
		    var postdata = { pessoa: $scope.pessoa, notafiscal: $scope.nf };//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };
		}

		$http.post("../../NFSeimportacao/" + metodo, postdata)
        .success(function (data) {

            if (data.CDStatus == "OK") {
                $scope.showImportacao = false;
                $scope.showMostraTodos = true;
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $location.hash('showOKTop');
                $scope.Pesquisa();
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
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";

            $scope.msgSalvar = "Salvar";
            $scope.btnSalvar = true;

        });
	}



	$scope.fcAdicionaParcela = function (obj) {

		$scope.MsgDivDanger = "";
		$scope.MsgDivSuccess = "";
		$scope.ShowDivValidacao = false;

		if ($scope.CPAGPARCELA.vencimento == null) {
		    $scope.MsgDivValidacao = "Informe o vencimento. ";
		    $scope.ShowDivValidacao = true;
		    return;
		}
		if ($scope.CPAGPARCELA.vencimentoPrevisto == null) {
		    $scope.MsgDivValidacao = "Informe o vencimento Previsto. ";
		    $scope.ShowDivValidacao = true;
		    return;
		}

		if ($scope.CPAGPARCELA.parcelas == null) {
		    $scope.MsgDivValidacao = "Informe a quantidade de parcelas.";
		    $scope.ShowDivValidacao = true;
		    return;
		}

		if ($scope.CPAGPARCELA.parcelas == 0) {
		    $scope.MsgDivValidacao = "Informe a quantidade de parcelas.";
		    $scope.ShowDivValidacao = true;
		    return;
		}

		if ($scope.CPAG.valorBruto == 0) {
		    $scope.MsgDivValidacao = "Informe o valor.";
		    $scope.ShowDivValidacao = true;
		    return;
		}

		$scope.CPAGPARCELAS = [];

		$http.post('../../CPAG/CalculaParcelas',
        {
            vencimento: $scope.CPAGPARCELA.vencimento,
            vencimentoPrevisto: $scope.CPAGPARCELA.vencimentoPrevisto,
            parcelas: $scope.CPAGPARCELA.parcelas,
            valorBruto: $scope.CPAG.valorBruto,
            historico: $scope.CPAGPARCELA.historico,
            sobra: $scope.CPAGPARCELA.sobra

        })
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.CPAGPARCELAS = data.CPAGPARCELAS;
                $scope.MsgDivSuccess = 'Salvo com sucesso.';
            } else {
                $scope.MsgDivDanger = data.DSMessage;
            }
            $scope.SomaTotalParcelas();

            Metronic.unblockUI();

        })
        .error(function (data) {
            $scope.MsgDivDanger = 'Sistema Indisponível';
            $scope.SomaTotalParcelas();
            Metronic.unblockUI();

        });



	}
	$scope.fcExcluirParcela = function (id) {
		$scope.CPAGPARCELAS.splice(id, 1);

		$scope.SomaTotalParcelas();
	}
	$scope.SomaTotalParcelas = function () {
		$scope.valorTotalParcelas = 0;

		angular.forEach($scope.CPAGPARCELAS, function (value) {
		    $scope.valorTotalParcelas += value.valor;
		});
	}
	$scope.cancel = function () {
		$scope.showImportacao = false;
		$scope.showMostraTodos = true;
		$scope.Pesquisa();
		$location.hash('DivTotal');
	};


	$scope.LoadListaPessoas = function () {

		//angular.forEach($scope.frmpessoas.$error.required, function (field) {
		//    field.$setDirty();
		//});

		$http.post("../../NFSeImportacao/ListaPessoas")
            .success(function (data) {
                $scope.ListaPessoas = data;
            });
	}

	$scope.LoadListaCategorias = function () {
		$http.post("../../NFSeImportacao/ListaCategorias")
            .success(function (data) {
                $scope.ListaCategorias = data;
            });
	}

	$scope.LoadListaUnidades = function () {
		$http.post("../../NFSeImportacao/ListaUnidades")
            .success(function (data) {
                $scope.ListaUnidadeNegocio = data;
            });
	}

	$scope.LoadListaTipoEnderecos = function () {
		$http.post("../../NFSeImportacao/ListaTipoEnderecos")
            .success(function (data) {
                $scope.ListaTipoEnderecos = data;
            });
	}


	$scope.LoadListaTipoDocumento = function () {
		$http.post("../../NFSeImportacao/ListaTipoDocumento")
            .success(function (data) {
                $scope.ListaTipoDocumento = data;
            });
	}

	$scope.LoadListaTipoLancamento = function () {
		$http.post("../../NFSeImportacao/ListaTipoLancamento")
            .success(function (data) {
                $scope.ListaTipoLancamento = data;
            });
	}

	$scope.LoadListaBanco = function () {
		$http.post("../../NFSeImportacao/ListaBanco")
            .success(function (data) {
                $scope.ListaBanco = data;
            });
	}

	$scope.LoadListaContaContabil = function () {
		$http.post("../../NFSeImportacao/ListaContaContabil")
            .success(function (data) {
                $scope.ListaContaContabil = data;
            });
	}

	$scope.LoadListaOperacao = function () {
		$http.post("../../NFSeImportacao/ListaOperacao")
            .success(function (data) {
                $scope.ListaOperacao = data;

            });
	}

	$scope.LoadPessoas = function () {
		$http.get("../../NFSeImportacao/ObterPessoaPorCNPJ/" + $scope.item.CPFCNPJPrestador.CNPJ)
        .success(function (data) {
            $scope.pessoa = data;
            if (data.id == 0) {
                $scope.pessoa.nome = $scope.item.RazaoSocialPrestador;
                $scope.pessoa.razao = $scope.item.RazaoSocialPrestador;
                $scope.pessoa.cnpj = $scope.item.CPFCNPJPrestador.CNPJ;
                $scope.pessoa.codigo = $scope.item.CPFCNPJPrestador.CNPJ;
                $scope.pessoa.inscricao = "Isento";
                $scope.pessoa.ccm = $scope.item.ChaveNFe.InscricaoPrestador;
                $scope.pessoa.endereco = $scope.item.EnderecoPrestador.Logradouro;
                $scope.pessoa.numero = $scope.item.EnderecoPrestador.NumeroEndereco;
                $scope.pessoa.complemento = $scope.item.EnderecoPrestador.ComplementoEndereco;
                $scope.pessoa.bairro = $scope.item.EnderecoPrestador.Bairro;
                $scope.pessoa.cidade = $scope.item.EnderecoPrestador.Cidade;
                $scope.pessoa.uf = $scope.item.EnderecoPrestador.UF;
                $scope.pessoa.cep = $scope.item.EnderecoPrestador.CEP.substr(0, 5) + $scope.item.EnderecoPrestador.CEP.substr(4, 3);
                $scope.pessoa.CategoriaPessoa_ID = "2";
                $scope.pessoa.TipoPessoa_ID = "68";

                if ($scope.item.EnderecoPrestador.TipoLogradouro == "AV")
                    $scope.pessoa.TipoEndereco_ID = "1";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "R")
                    $scope.pessoa.TipoEndereco_ID = "2";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "AL")
                    $scope.pessoa.TipoEndereco_ID = "3";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "TR")
                    $scope.pessoa.TipoEndereco_ID = "4";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "VI")
                    $scope.pessoa.TipoEndereco_ID = "43";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "ES")
                    $scope.pessoa.TipoEndereco_ID = "44";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "SE")
                    $scope.pessoa.TipoEndereco_ID = "45";
                else if ($scope.item.EnderecoPrestador.TipoLogradouro == "ES")
                    $scope.pessoa.TipoEndereco_ID = "47";



            }

        }).error(function (data) {
            $scope.MsgError("Sistema indisponivel");
        });
	}

	$scope.LoadNotaFiscal = function () {
		$http.post("../../NFSeImportacao/ObterPorCodigoVerificacao", { Identificador: $scope.item.ChaveNFe.CodigoVerificacao })
        .success(function (data) {
            $scope.nf = data;
            if (data.id == 0) {
                $scope.nf.numeroRps = $scope.item.ChaveRPS.NumeroRPS;
                $scope.nf.tipoRps = "1";
                $scope.nf.serieRps = $scope.item.ChaveRPS.SerieRPS;
                $scope.nf.numeroNfse = $scope.item.ChaveNFe.NumeroNFe;
                $scope.nf.dataEmissaoNfse = $scope.item.DataEmissaoNFe;
                $scope.nf.dataVencimentoNfse = $scope.item.DataEmissaoNFe;
                $scope.nf.dataEmissaoRps = $scope.item.DataEmissaoRPS;
                $scope.nf.discriminacaoServico = $scope.item.Discriminacao;
                $scope.nf.valorNfse = $scope.item.ValorServicos;
                $scope.nf.codigoVerificacao = $scope.item.ChaveNFe.CodigoVerificacao;
                $scope.nf.codigoServico = $scope.item.CodigoServico;
                $scope.nf.valorDeducoes = 0;
                $scope.nf.basedeCalculo = 0;
                $scope.nf.aliquotaISS = 0;
                $scope.nf.creditoImposto = 0;
                $scope.nf.irrf = 0;
                $scope.nf.aliquotaIrrf = 0;
                $scope.nf.pisRetido = 0;
                $scope.nf.cofinsRetida = 0;
                $scope.nf.valorLiquido = 0;

                $scope.nf.NotaFiscalPessoaPrestador.razao = $scope.item.RazaoSocialPrestador;
                $scope.nf.NotaFiscalPessoaPrestador.bairro = $scope.item.EnderecoPrestador.Bairro;
                $scope.nf.NotaFiscalPessoaPrestador.cidade = $scope.item.EnderecoPrestador.Cidade;
                $scope.nf.NotaFiscalPessoaPrestador.cep = $scope.item.EnderecoPrestador.CEP;
                $scope.nf.NotaFiscalPessoaPrestador.complemento = $scope.item.EnderecoPrestador.ComplementoEndereco;

                if ($scope.item.CPFCNPJPrestador.CNPJ != null)
                {
                    $scope.nf.NotaFiscalPessoaPrestador.cnpjCpf = $scope.item.CPFCNPJPrestador.CNPJ;
                    $scope.nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf = 2;
                    $scope.nf.NotaFiscalPessoaPrestador.inscricaoEstadual = $scope.item.ChaveNFe.InscricaoPrestador;
                }
                else
                {
                    $scope.nf.NotaFiscalPessoaPrestador.cnpjCpf = $scope.item.CPFCNPJPrestador.CPF;
                    $scope.nf.NotaFiscalPessoaPrestador.indicadorCnpjCpf = 1;
                }
                $scope.nf.NotaFiscalPessoaPrestador.email = "-";
                $scope.nf.NotaFiscalPessoaPrestador.endereco = $scope.item.EnderecoPrestador.Logradouro;
                $scope.nf.NotaFiscalPessoaPrestador.numero = $scope.item.EnderecoPrestador.NumeroEndereco;
                $scope.nf.NotaFiscalPessoaPrestador.tipoEndereco = $scope.item.EnderecoPrestador.TipoLogradouro;
                $scope.nf.NotaFiscalPessoaPrestador.uf = $scope.item.EnderecoPrestador.UF;
                $scope.nf.NotaFiscalPessoaPrestador.id = 0;
            }

            $scope.ObterCPAG();

        }).error(function (data) {
            $scope.MsgError("Sistema indisponivel");
        });
	}

	$scope.CalculaNota = function () {
		$http.post("../../NFSeImportacao/CalculaNota", { idOperacao: $scope.nf.operacao_id, valorNota: $scope.nf.valorNfse })
            .success(function (data) {
                $scope.nf.valorDeducoes = data.valorDeducoes;
                $scope.nf.basedeCalculo = data.basedeCalculo;
                $scope.nf.aliquotaISS = data.aliquotaISS;
                $scope.nf.creditoImposto = data.creditoImposto;
                $scope.nf.irrf = data.irrf;
                $scope.nf.aliquotaIrrf = data.aliquotaIrrf;
                $scope.nf.valorINSS = data.valorINSS;
                $scope.nf.aliquotaINSS = data.aliquotaINSS;
                $scope.nf.pisRetido = data.pisRetido;
                $scope.nf.cofinsRetida = data.cofinsRetida;
                $scope.nf.valorLiquido = data.valorLiquido;
                $scope.nf.csllRetida = data.csllRetida;

            });
	}

	$scope.ObterCPAG = function () {
		$http.post("../../NFSeImportacao/ObterCPAGCodigoVerificacao", { Identificador: $scope.item.ChaveNFe.CodigoVerificacao })
            .success(function (data) {
                $scope.CPAG = data.CPAG;
                $scope.CPAGITENS = data.CPAGITENS;

                $scope.CPAG.dataVencimento = $scope.item.DataEmissaoNFe;;
                $scope.CPAG.dataDocumento = $scope.item.DataEmissaoNFe;;
                $scope.CPAG.dataVencimentoOriginal = $scope.item.DataEmissaoNFe;
                $scope.CPAGPARCELA = data.CPAGPARCELA;

            });
	}

	$scope.fcAdiciona = function (obj) {
		$scope.MsgDivDanger = "";
		$scope.MsgDivSuccess = "";
		$scope.ShowDivValidacao = false;
		//$scope.SomaTotal();


		if ($scope.CPAG.valorBruto == $scope.valorTotal) {
		    $scope.MsgDivValidacao = "Valor total atingido. ";
		    $scope.ShowDivValidacao = true;
		    return;
		}


		if ($scope.CPAG.valorBruto == 0) {
		    $scope.MsgDivValidacao = "Informe o valor bruto. ";
		    $scope.ShowDivValidacao = true;
		    return;
		}


		if (obj.valor > $scope.CPAG.valorBruto) {
		    $scope.MsgDivValidacao = "Valor bruto não pode ser menor que o informado na parcela. ";
		    $scope.ShowDivValidacao = true;
		    return;
		}
		if (obj.valor == 0) {
		    if (obj.PorcentagemLct == 0) {
		        $scope.MsgDivValidacao = "Informe o valor ou a porcentagem. ";
		        $scope.ShowDivValidacao = true;
		        return;
		    }
		    if (obj.PorcentagemLct > 100) {
		        $scope.MsgDivValidacao = "Porcentagem tem que estar entre 0 e 100.";
		        $scope.ShowDivValidacao = true;
		        return;
		    }

		    obj.valor = parseFloat((($scope.CPAG.valorBruto / 100) * obj.PorcentagemLct).toFixed(2));

		    if ((obj.valor + $scope.valorTotal) > $scope.CPAG.valorBruto) {
		        $scope.MsgDivValidacao = "Valor supera 100%.";
		        $scope.ShowDivValidacao = true;
		        return;
		    }

		} else {
		    if ((obj.valor + $scope.valorTotal) > $scope.CPAG.valorBruto) {
		        $scope.MsgDivValidacao = "Valor supera 100%.";
		        $scope.ShowDivValidacao = true;
		        return;
		    }

		}

		if ($scope.CPAGITEM.UnidadeNegocio_desc == null) {
		    $scope.MsgDivValidacao = "Informe a Unidade.";
		    $scope.ShowDivValidacao = true;
		    return;
		}

		if ($scope.CPAGITEM.historico == null) {
		    $scope.MsgDivValidacao = "Informe o histórico.";
		    $scope.ShowDivValidacao = true;
		    return;
		}


		$scope.objAux = {};
		angular.copy(obj, $scope.objAux);
		if ($scope.CPAGITEMS == null)
		    $scope.CPAGITEMS = [];

		$scope.CPAGITEMS.push($scope.objAux);

		$scope.SomaTotal();


	}
	$scope.fcExcluir = function (id) {
		$scope.CPAGITEMS.splice(id, 1);

		$scope.SomaTotal();
	}
	$scope.SomaTotal = function () {
		$scope.valorTotal = 0;

		angular.forEach($scope.CPAGITEMS, function (value) {
		    $scope.valorTotal += value.valor;
		});
	}



	$scope.AtualizaCPAG = function () {
		$scope.CPAG.valorBruto = $scope.nf.valorLiquido;
		$scope.CPAG.banco_id = $scope.nf.banco_id;
		$scope.CPAG.dataVencimento = $scope.nf.dataVencimentoNfse;
		$scope.CPAG.dataDocumento = $scope.nf.dataVencimentoNfse;
		$scope.CPAG.dataVencimentoOriginal = $scope.nf.dataVencimentoNfse;
		$scope.CPAG.nome = $scope.item.RazaoSocialPrestador;
		$scope.CPAGITEM = {
		    'valor': 0,
		    'historico': 0
		};

		$scope.CPAGITEM.historico = "NF importada";
		$scope.CPAGITEM.valor = $scope.nf.valorLiquido;
	};

    
	$scope.LoadListaPessoas();
	$scope.LoadListaCategorias();
	$scope.LoadListaUnidades();
	$scope.LoadListaTipoEnderecos();
		    
	//Contas a Pagar
	$scope.LoadListaTipoDocumento();
	$scope.LoadListaBanco();
	$scope.LoadListaContaContabil();
	$scope.LoadListaTipoLancamento();
		    
	//$scope.LoadListaCodigoServico();
	$scope.LoadListaOperacao();


	}
]);

