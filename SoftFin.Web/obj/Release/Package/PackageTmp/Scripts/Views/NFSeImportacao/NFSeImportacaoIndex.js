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
		    $scope.showGrid = false;
		    $scope.showloadGrid = false;
		    //$scope.frmpessoas.$setSubmitted();

		    Metronic.blockUI({
		        boxed: true
		    });


		    

		    $scope.ordenar = function (keyname) {
		        $scope.sortKey = keyname;
		        $scope.reverse = !$scope.reverse;
		    };
		    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
		    $scope.ListaSobra = [{ Value: "U", Text: "Última Parcela" }, { Value: "P", Text: "Primeira Parcela" }];
		    $scope.ListaInstituicao = [{ Value: "Prefeitura", Text: "Prefeitura" }];

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
		        $scope.ListaNotas = [];
		        $scope.ListaNotas = "";
		        $scope.showGrid = false;
		        $scope.showloadGrid = true;
		        $http.post("../../NFSeImportacao/ListaNotas", { instituicao: $scope.filtro.instituicao, dataInicial: $scope.filtro.dataInicial, dataFinal: $scope.filtro.dataFinal })
                    
					.success(function (data) {
					    if (data.CDStatus != "NOK") {
					        $scope.ListaNotas = data.NFSe;
					        $scope.showGrid = true;
					        $scope.showloadGrid = false;
					    }
					    else {
					        $scope.MsgError("Erro ao carregar as notas");
					        $scope.Alertas = data.Alertas;
					        $scope.Erros = data.Erros;
					        $scope.ListaNotas = [];
					        $scope.showloadGrid = false;

					    }
					}).error(function (data) {
					    $scope.MsgError("Sistema indisponivel");
					    $scope.ListaNotas = [];
					    $scope.showloadGrid = false;
					});
		    }

		    $scope.Importa = function (item) {
		        $location.hash('divImportacao');
		        $scope.pessoa = "";
		        $scope.nf = "";
		        $scope.CPAG = "";
		        $scope.CPAGITEMS = "";
		        $scope.GERACPAG = false;
		        $scope.CPAGITEMS = [];
		        

		        $('#chkImp').attr('checked', false);

		        $scope.item = item;
		        $scope.idSelecionado = item.ChaveNFe.CodigoVerificacao;
		        console.log(item);
		        $scope.nf.dataEmissaoNfse = item.DataEmissaoNFe;
		        $scope.showMostraTodos = false;
		        $scope.LoadPessoas();
		        $scope.LoadNotaFiscal();
		        $scope.showImportacao = true;

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


		    //$scope.LoadListaCodigoServico = function () {
		    //    $http.post("../../NFSeImportacao/ListaCodigoServico")
            //        .success(function (data) {
            //            $scope.ListaCodigoServico = data;
            //        });
		    //}


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


    var ControllerNF = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    }
