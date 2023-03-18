
$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);

function redimensionaImagens() {
    elem = document.getElementById("imgcabec");
    if (elem.offsetWidth > 700) {
        elem.style.height = Math.round(elem.offsetHeight * (700 / elem.offsetWidth));
    }
}

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
        $scope.MsgLinha = "";
        $scope.ShowDivValidacao = "";
        $scope.MsgDivValidacaoParcela = "";
        $scope.carregados = 0;
        $scope.naocarregados = 0;
        $scope.showupload = 0;

        $scope.EditAccess = false;
        $http.post("../../CPAG/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        $scope.uploadResultado = function () {
            $timeout(
                function () {
                    $scope.showupload = 0;
   
                }, 8000);
        };
        


        $scope.ListaSobra = [{ Value: "U", Text: "Última Parcela" }, { Value: "P", Text: "Primeira Parcela" }];


        $scope.procurar = "";
        $scope.sortKey = "";
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        $scope.showGrid = true;
        $scope.ModoConsulta = false;
        $scope.ModoDelete = false;
        $scope.TitulobtnOK = "OK";
        $scope.ShowbtnOK = false;
        $scope.valorTotal = 0;
        $scope.valorTotalParcelas = 0;
        $scope.ValorTotalProjetos = 0;
        $scope.id = 0;
        $scope.showDivGPS = true;
        $scope.showDivDARF = true;
        $scope.showDivFGTS = true;
        $scope.ShowDivValidacao = false;
        $scope.showCadastro = false;
        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";

        $scope.carregaCPAGITEM = function (id) {
            $scope.CPAGITEM = {
                'id': 0,
                'ContaContabil': null,
                'UnidadeNegocio': null,
                'valor': 0,
                'PorcentagemLct': 0,
                'historico': null
            };
        };

        $scope.carregaCPAGITEM();

        $scope.CPAGPARCELA = {
            'id': 0,
            'vencimento': null,
            'vencimentoPrevisto': null,
            'parcelas': 0,
            'historico': null,
            'sobra': null
        };

        $scope.PROJETOITEM = {"Id":0, "Historico":"", "Valor":0,"Projeto_Id": 0};


        $scope.LoadObtemEntidade = function (id, copiar) {
            $scope.CPAG = "";
            $scope.CPAGITEMS = "";
            $scope.GPS = "";
            $scope.DARF = "";
            $scope.FGTS = "";
            $scope.PROJETOITEM = "";


            $http.post("../../CPAG/ObtemEntidade", "{id:" + id + ", copiar:" + copiar + "}")
                .success(function (data) {

                    $scope.CPAG = data.CPAG;
                    $scope.CPAGITEMS = data.CPAGITENS;
                    $scope.CPAGPARCELAS = data.CPAGPARCELAS;
                    $scope.CPAGPARCELA = data.CPAGPARCELA;
                    $scope.PROJETOITEMS = data.PROJETOSITEMS;


                    if ($scope.CPAG.tipodocumento_id == 66)
                        $scope.GPS = data.GPS;

                    if ($scope.CPAG.tipodocumento_id == 67)
                        $scope.DARF = data.DARF;

                    if ($scope.CPAG.tipodocumento_id == 68)
                        $scope.FGTS = data.FGTS;

                    $scope.MostraComplementoTipoDOC(false);
                    $scope.SomaTotal();
                    $scope.showCadastro = true;
                    $scope.showGrid = false;

                });
        }

        $scope.MostraComplementoTipoDOC = function (carregaTela) {
            $scope.showDivFGTS = false;
            $scope.showDivDARF = false;
            $scope.showDivGPS = false;
            if ($scope.CPAG.tipodocumento_id == 68) {

                $scope.showDivFGTS = true;
                //if (carregaTela == true) {
                //    $scope.FGTS.nomeContribuinte = $scope.CPAG.pessoa_id.split(',')[0];
                //    $scope.FGTS.cnpj = $scope.CPAG.pessoa_id.split(',')[1];

                //}
            }
            if ($scope.CPAG.tipodocumento_id == 67) {
                $scope.showDivDARF = true;
                //if (carregaTela == true) {
                //    $scope.DARF.periodoApuracao = $scope.CPAG.dataCompetencia;
                //    $scope.DARF.valorPrincipal = $scope.CPAG.valorBruto;
                //    $scope.DARF.valorTotal = $scope.CPAG.valorBruto;
                //}
            }
            if ($scope.CPAG.tipodocumento_id == 66) {
                $scope.showDivGPS = true;
                //if (carregaTela == true) {

                //}

            }

        }


        $scope.OK = function () {
            
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            if ($scope.ModoDelete == true) {
                $scope.Excluir();
            }
            else {
                $scope.Salvar();
            }
        }

        $scope.Salvar = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";

            if ($scope.frmDadosGerais.$valid) {
                $scope.CPAGAux = { cpag: $scope.CPAG, cpagItems: $scope.CPAGITEMS, cpagParcelas: $scope.CPAGPARCELAS, fgts: null, darf: null, gps: null, 'projetos': $scope.PROJETOITEMS };

                if ($scope.CPAG.tipodocumento_id == 68) {
                    if ($scope.frmFGTS.$valid) {
                        $scope.CPAGAux = { cpag: $scope.CPAG, cpagItems: $scope.CPAGITEMS, cpagParcelas: $scope.CPAGPARCELAS, fgts: $scope.FGTS, darf: null, gps: null, 'projetos': $scope.PROJETOITEMS };
                    }
                    else {
                        $scope.MsgDivDanger = 'Informe os campos obrigatórios de FGTS.';
                        return;
                    }
                }
                if ($scope.CPAG.tipodocumento_id == 67) {
                    if ($scope.frmDARF.$valid) {
                        $scope.CPAGAux = { cpag: $scope.CPAG, cpagItems: $scope.CPAGITEMS, cpagParcelas: $scope.CPAGPARCELAS, darf: $scope.DARF, fgts: null, gps: null, 'projetos': $scope.PROJETOITEMS};

                    }
                    else {
                        $scope.MsgDivDanger = 'Informe os campos obrigatórios de DARF.';
                        return;
                    }
                }
                if ($scope.CPAG.tipodocumento_id == 66) {
                    if ($scope.frmGPS.$valid) {
                        $scope.CPAGAux = { cpag: $scope.CPAG, cpagItems: $scope.CPAGITEMS, cpagParcelas: $scope.CPAGPARCELAS, gps: $scope.GPS, fgts: null, darf: null, 'projetos': $scope.PROJETOITEMS };
                    }
                    else {
                        $scope.MsgDivDanger = 'Informe os campos obrigatórios de GPS.';
                        return;
                    }
                }

                if ($scope.MsgLinha != "") {
                    $scope.MsgDivDanger = $scope.MsgLinha;
                    return;
                }

                Metronic.blockUI({
                    boxed: true
                });

                $http.post('../../CPAG/Salvar', $scope.CPAGAux)
                .success(function (data) {
                    if (data.CDMessage == "OK") {

                        $scope.Pesquisar(false);
                        $scope.MsgDivSuccess = 'Salvo com sucesso.';
                    } else {
                        $scope.MsgDivDanger = data.DSErroReturn;
                    }
                    Metronic.unblockUI();

                })
                .error(function (data) {
                    $scope.MsgDivDanger = 'Sistema Indisponível';
                    Metronic.unblockUI();

                });
            } else {
                $scope.MsgDivDanger = 'Informe os campos obrigatórios.';
            }

        }



        $scope.Loader = function (isCarregar, msg) {
            if (msg == undefined) { msg = "Aguarde, carregando..."; }
            $scope.ShowLoad = isCarregar;
            $scope.MsgDivLoad = msg;
        }

        $scope.Loader(false);

        $scope.LoadProjetos = function () {
            $http.post("../../Projetos/ObterProjetos")
                .success(function (data) {
                    $scope.ListaProjetosDrop = data;
                });
        };
        $scope.LoadProjetos();


        $scope.LoadListaTipoDocumento = function () {
            $http.post("../../CPAG/ListaTipoDocumento")
                .success(function (data) {
                    $scope.ListaTipoDocumento = data;
                });
        }

        $scope.LoadListaTipoLancamento = function () {
            $http.post("../../CPAG/ListaTipoLancamento")
                .success(function (data) {
                    $scope.ListaTipoLancamento = data;
                });
        }

        $scope.LoadListaBanco = function () {
            $http.post("../../CPAG/ListaBanco")
                .success(function (data) {
                    $scope.ListaBanco = data;
                });
        }

        $scope.LoadListaContaContabil = function () {
            $http.post("../../CPAG/ListaContaContabil")
                .success(function (data) {
                    $scope.ListaContaContabil = data;
                });
        }

        $scope.LoadListaUnidadeNegocio = function () {
            $http.post("../../CPAG/ListaUnidadeNegocio")
                .success(function (data) {
                    $scope.ListaUnidadeNegocio = data;
                });
        }

        $scope.LoadListaCliente = function () {
            $http.post("../../CPAG/ListaCliente")
                .success(function (data) {
                    $scope.ListaCliente = data;
                });
        }

        $scope.LoadListaFornecedor = function () {
            $http.post("../../CPAG/ListaFornecedor")
                .success(function (data) {
                    $scope.ListaFornecedor = data;
                });
        }

        $scope.LoadListaPessoas = function () {
            $http.post("../../CPAG/ListaPessoas")
                .success(function (data) {
                    $scope.ListaPessoas = data;
                });
        }

        $scope.ConfiguraFiltro = function () {
            $http.get("../../CPAG/obtemFiltro")
            .success(function (data) {
                $scope.filtro = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                $scope.ListaNotas = [];
                Metronic.unblockUI();
            });
        }

        $scope.AdicionaProjeto = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ShowDivValidacaoProjeto = false;


            if ($scope.PROJETOITEM.Valor == 0) {
                $scope.MsgDivValidacaoProjeto = "Informe o valor do projeto. ";
                $scope.ShowDivValidacaoProjeto = true;
                return;
            }



            

            if ($scope.PROJETOITEM.Valor > $scope.CPAG.valorBruto) {
                $scope.MsgDivValidacaoProjeto = "Valor do Projeto não pode ser maior que o do Contas a Pagar. ";
                $scope.ShowDivValidacaoProjeto = true;
                return;
            }

            if (($scope.PROJETOITEM.Valor + $scope.ValorTotalProjetos) > $scope.CPAG.valorBruto) {
                $scope.MsgDivValidacaoProjeto = "Valor do Projeto supera o do Contas a Pagar. ";
                $scope.ShowDivValidacaoProjeto = true;
                return;
            }

            if ($scope.PROJETOITEM.Projeto_Id == null) {
                $scope.MsgDivValidacaoProjeto = "Informe o Projeto.";
                $scope.ShowDivValidacaoProjeto = true;
                return;
            }


            if ($scope.PROJETOITEM.Projeto_Id == 0) {
                $scope.MsgDivValidacaoProjeto = "Informe o Projeto.";
                $scope.ShowDivValidacaoProjeto = true;
                return;
            }

            for (var i = 0; i < $scope.ListaProjetosDrop.length; i++) {
                if ($scope.ListaProjetosDrop[i].Value == $scope.PROJETOITEM.Projeto_Id)
                    $scope.PROJETOITEM.ProjetoNome = $scope.ListaProjetosDrop[i].Text;
            }

            if ($scope.PROJETOITEMS == null)
                $scope.PROJETOITEMS = [];
            $scope.objAux = {};
            angular.copy($scope.PROJETOITEM, $scope.objAux);
            $scope.PROJETOITEMS.push($scope.objAux);

            $scope.SomaTotal();


        }
        $scope.ExcluirProjeto = function (id) {
            $scope.PROJETOITEMS.splice(id, 1);

            $scope.SomaTotal();
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

            if ($scope.CPAGITEM.historico == "") {
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

            $scope.ValorTotalProjetos = 0;

            angular.forEach($scope.PROJETOITEMS, function (value) {
                $scope.ValorTotalProjetos += value.Valor;
            });
        }





        $scope.fcAdicionaParcela = function (obj) {

            $scope.ShowDivValidacao = "";
            $scope.MsgDivValidacaoParcela = "";
                
            if ($scope.CPAGPARCELA.vencimento == null) {
                $scope.MsgDivValidacaoParcela = "Informe o vencimento. ";
                return;
            }
            if ($scope.CPAGPARCELA.vencimentoPrevisto == null) {
                $scope.MsgDivValidacaoParcela = "Informe o vencimento Previsto. ";
                return;
            }

            if ($scope.CPAGPARCELA.parcelas == null) {
                $scope.MsgDivValidacaoParcela = "Informe a quantidade de parcelas.";
                return;
            }

            if ($scope.CPAGPARCELA.parcelas == 0) {
                $scope.MsgDivValidacaoParcela = "Informe a quantidade de parcelas.";
                return;
            }

            if ($scope.CPAG.valorBruto == 0) {
                $scope.MsgDivValidacaoParcela = "Informe o valor.";
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
                    $scope.ShowDivValidacao = 'Salvo com sucesso.';
                } else {
                    $scope.MsgDivValidacaoParcela = data.DSMessage;
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

        $scope.CodigoBarras = function () {
            if ($scope.CPAG.LinhaDigitavel == "") {
                $scope.MsgLinha = "";
            }
            else {
                $http.post("../../CPAG/codigoBarras", "{LinhaDigitavel:'" + $scope.CPAG.LinhaDigitavel + "'}")
                    .success(function (data) {
                        $scope.MsgLinha = data.Erro;
                        if (data.Erro == "") {
                            
                            $scope.CPAG.LinhaDigitavel = data.LinhaDigitavel;
                            $scope.CPAGPARCELA.vencimentoPrevisto = data.Vencimento;
                            $scope.CPAGPARCELA.vencimento = data.Vencimento;
                            $scope.CPAG.valorBruto = data.Valor;
                            $scope.CPAGITEM.valor = data.Valor;
                            $scope.PROJETOITEM.Valor = data.Valor; 
                            $scope.CPAG.tipodocumento_id = "62";
                        };
                    });
            }
        }


        $scope.Pesquisar = function (travatela) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";

            if (travatela) {
                Metronic.blockUI({
                    boxed: true
                });
            }

            $http.post("../../CPAG/ObtemTodos", $scope.filtro)
                .success(function (data) {
                    if (travatela) {
                        Metronic.unblockUI();
                    }
                    $scope.ListaCPAG = data;
                });
        }

        $scope.Incluir = function () {
            $scope.MsgLinha = "";
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.valorTotalParcelas = 0;
            $scope.carregaCPAGITEM();
            $scope.LoadObtemEntidade(0,"false");
        }

        $scope.Voltar = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.showCadastro = false;
            $scope.showGrid = true;
            
        }
        $scope.Editar = function (item) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ModoConsulta = false;
            $scope.LoadObtemEntidade(item.id, "false");
        }
        $scope.Detalhar = function (item) {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ModoConsulta = true;
            $scope.LoadObtemEntidade(item.id, "false");
        }
        $scope.Copiar = function (item) {
            $scope.MsgLinha = "";
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ModoConsulta = false;
            $scope.LoadObtemEntidade(item.id, "true");
        }
        Metronic.blockUI({
            boxed: true
        });

        $scope.Excluir = function (item) {
            $scope.MsgLinha = "";
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
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
                $scope.Pesquisar(true);
            }, function () {
            });
        }
        //fcEditarParcela
        $scope.fcEditarParcela = function (index) {

            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: '../../page/CPAG/editarParcela.html',
                controller: ControllerEditarParcela,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.CPAGPARCELAS[index] }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.CPAGPARCELAS[index] = value;
                $scope.SomaTotalParcelas();
            }, function () {
                $scope.SomaTotalParcelas();
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

                if (data.CDStatus == "OK") {
                    $scope.CPAG = data.CPAG;
                    $scope.CPAGPARCELA = data.CPAGPARCELA;
                    $scope.CPAGPARCELAS = data.CPAGPARCELAS;
                    $scope.CPAGITEMS = [];
                    $scope.CPAGITEM.valor = $scope.CPAG.valorBruto
                    $scope.CPAGITEM.historico = $scope.CPAG.descricaoExtra;

                    if ($scope.CPAG.tipodocumento_id == 66)
                        $scope.GPS = data.GPS;

                    if ($scope.CPAG.tipodocumento_id == 67) {
                        $scope.DARF = data.DARF;
                    }

                    if ($scope.CPAG.tipodocumento_id == 68)
                        $scope.FGTS = data.FGTS;

                    $scope.MostraComplementoTipoDOC(false);
                    $scope.SomaTotal();
                    $scope.showCadastro = true;
                    $scope.showGrid = false;
                    $scope.ModoConsulta = false;
                    redimensionaImagens();

                }

            }, function () {
                //on cancel button press
                //console.log("Modal Closed");
            });
        };


        $scope.ObterConsultaLC = function (item) {

            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../LancamentoContabil/ObterConsulta", { IdCPAG: item.id, idNF: 0} )
                .success(function (data) {
                    $scope.ListaCC = data;

                    $scope.opts = {
                        size: 'lg',
                        animation: true,
                        backdrop: false,
                        backdropClick: false,
                        dialogFade: true,
                        keyboard: true,
                        templateUrl: '../../page/contacontabil/consulta.html',
                        controller: ControllerLC,
                        resolve: {} // empty storage
                    };

                    $scope.opts.resolve.item = function () {
                        return angular.copy({ item: $scope.ListaCC }); // pass name to Dialog
                    }

                    var modalInstance = $modal.open($scope.opts);

                    modalInstance.result.then(function () {

                    }, function () {
                        
                    });

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



        $scope.Venctos = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalVencimentos.html',
                controller: ControllerVencimentos,
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


        //read object properties and sum price
        $scope.calculateSum = function (data) {
            var sum = 0;
            var counter = 0;
            for (var property in data) {
                if (data.hasOwnProperty(property)) {
                    sum += data[property].valorBruto;
                    counter++;
                }
            }
            return sum;
        };




        $scope.ConfiguraFiltro();
        $scope.LoadListaTipoDocumento();
        $scope.LoadListaBanco();
        $scope.LoadListaCliente();
        $scope.LoadListaFornecedor();
        $scope.LoadListaPessoas();
        $scope.LoadListaContaContabil();
        $scope.LoadListaTipoLancamento();
        $scope.LoadListaUnidadeNegocio();

    }
]).directive('fileDropzone', function ($http, $timeout) {
    return {
        restrict: 'AEC',
        scope: {
            carregados: '=',
            naocarregados: '=',
            showupload: '='
        },
        link: function (scope, element, attrs) {
            var checkSize,
                isTypeValid,
                processDragOverOrEnter,
                validMimeTypes;



            processDragOverOrEnter = function (event) {
                if (event != null) {
                    event.preventDefault();
                }
                event.originalEvent.effectAllowed = "copy";
                return false;
            };

            validMimeTypes = attrs.fileDropzone;

            checkSize = function (size) {
                var _ref;
                if (((_ref = attrs.maxFileSize) === (void 0) || _ref === '') || (size / 1024) / 1024 < attrs.maxFileSize) {
                    return true;
                } else {
                    alert("File must be smaller than " + attrs.maxFileSize + " MB");
                    return false;
                }
            };

            isTypeValid = function (type) {
                if ((validMimeTypes === (void 0) || validMimeTypes === '') || validMimeTypes.indexOf(type) > -1) {
                    return true;
                } else {
                    alert("Invalid file type.  File must be one of following types " + validMimeTypes);
                    return false;
                }
            };

            element.bind('dragover', processDragOverOrEnter);
            element.bind('dragenter', processDragOverOrEnter);

            return element.bind('drop', function (event) {
                var file, name, reader, size, type;
                if (event != null) {
                    event.preventDefault();
                }
                reader = new FileReader();
                reader.onload = function (evt) {
                    if (checkSize(size) && isTypeValid(type)) {
                        return scope.$apply(function () {
                            scope.file = evt.target.result;
                            if (angular.isString(scope.fileName)) {
                                return scope.fileName = name;
                            }
                        });
                    }
                };


                scope.carregados = 0;
                scope.naocarregados = 0;

                function carregaArquivo(arquivos, index, total) {
                    
                    file = arquivos[index];

                    var descricao = "Upload de Arquivo";
                    var f = file;
                    var fd = new FormData();
                    fd.append("file", f);
                    fd.append("id", attrs.idcpag);
                    fd.append("descricao", descricao);

                    $http.post("../../../CPAG/Upload", fd, {
                        withCredentials: true,
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
                    }).success(function (data) {

                        if (data.CDStatus == "OK") {
                            scope.carregados += 1;
                        } else {
                            scope.naocarregados += 1;
                        };

                        if ((total - 1) == index) {
                            $timeout(function () {
                                scope.$parent.item.showupload = 1;
                                scope.$parent.item.carregados = scope.carregados;
                                scope.$parent.item.naocarregados = scope.naocarregados;
                                scope.$parent.item.qtdImagens += scope.carregados ;
                            });
                        }
                        else {
                            carregaArquivo(arquivos, index + 1, total);
                        }

                    }).error(function (data) {
                        $scope.showNOK = true;
                        $scope.msgNOK = "Sistema indisponivel";
                    });
                }


                arqs = event.originalEvent.dataTransfer.files;
                if (arqs.length > 0) {
                    carregaArquivo(arqs, 0, arqs.length);
                }
                //reader.readAsDataURL(file);
                return true;
            });
        }
    };
})


    .directive("fileread", [function () {
        return {
            scope: {
                fileread: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {
                    var reader = new FileReader();
                    reader.onload = function (loadEvent) {
                        scope.$apply(function () {
                            scope.fileread = loadEvent.target.result;
                        });
                    }
                    reader.readAsDataURL(changeEvent.target.files[0]);
                });
            }
        }
    }]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $scope.MsgDivDanger = "";
        console.log($scope.item.id);
        $http.post("../../CPAG/Excluir", "{id :" + $scope.item.id + "}")
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
                    $modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                    $scope.Erros = data.Erros;
                }
            })
        .error(function (data) {
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";

        });
    }


    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


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
        fd.append("id", $scope.item.item.id);
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

    $scope.carregaArquivos = function () {

        $http.post("../../../CPAG/Arquivos", { id: $scope.item.id }).success(function (data) {
            $scope.ListaArquivos = data;
            $scope.showOK = true;

        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.Excluir = function (item) {
        $http.post("../../../CPAG/RemoveArquivo", { id: item.id }).success(function (data) {
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


var ControllerUploadLacto = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

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
        fd.append("descricao", descricao);
        console.log(fd);
        console.log($scope.item.item);

        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../../CPAG/UploadLacto", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            };

            Metronic.unblockUI();

            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);


        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}




var ControllerEditarParcela = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;

    $scope.someMsg = function () {
        $scope.showNOK = false;
        $scope.showOK = false;
    };

    $scope.ok = function () {
        $modalInstance.close($scope.entidade);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerLC = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.procurar = "";
    $scope.ListaCC = item.item;
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


var ControllerVencimentos = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.showSalvar = true;
    $scope.item = item.item;

    $scope.carrega = function () {

        $http.post("../../../CPAG/ObtemVenctos", { idDocumento: $scope.item.id }).success(function (data) {
            $scope.objs = data.objs;
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.ok = function (item) {
        Metronic.blockUI({
            boxed: true
        });
        $scope.showOK = false;
        $scope.showSalvar = false;
        $http.post("../../../CPAG/SalvaVenctos", { objs: $scope.objs }).success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus == "OK") {

                $scope.showSalvar = true;
                $scope.showOK = true;
                $scope.msgOK = "Salvo com sucesso";
                $scope.carregaArquivos();
            } else {
                $scope.showSalvar = true;
                $scope.objs = data.objs;
                $scope.showNOK = true;
                $scope.msgNOK = "Verifique os erros.";
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
    $scope.carrega();
}

