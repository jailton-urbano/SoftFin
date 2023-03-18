    $(document).ready(function () {
        Metronic.init(); // init metronic core components
        Layout.init(); // init current layout
        QuickSidebar.init(); // init quick sidebar
    });

var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination', 'mentio']);
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
        return input == 'true' ? 'Sim' : 'Não';
    }
})

var ListaImpostos = [];

app.filter('ftimposto', function () {
    return function (input) {
        for (var i = 0; i < ListaImpostos.length; i++) {
            if (ListaImpostos[i].Value == input) {
                return ListaImpostos[i].Text;
            }
        }
        return "Não Encontrado";
    }
})


app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../MT/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });
		    
        $scope.idSelecionado = 0
        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        $scope.ModoConsulta = false;
        $scope.ShowServico = true;


        $scope.hashtag = [
            { label: 'Nota' },
            { label: 'Mike' },
            { label: 'Diane' }
        ];

        $scope.ListaTributacao = [
                            { Value: 'T', Text: 'T - Tributado' },
                            { Value: 'I', Text: 'I - Isento' },
                            { Value: 'O', Text: 'O - Outras' },
                            { Value: 'S', Text: 'S - Suspenso' },
                            { Value: 'D', Text: 'D - Diferido' },
                            { Value: 'N', Text: 'N - Não Tributado' }
        ];

        $scope.ListaCSOSN = [
                            { Value: '101', Text: '101 Tributada pelo Simples Nacional com permissão de crédito' },
                            { Value: '102', Text: '102 Tributada pelo Simples Nacional sem permissão de crédito' },
                            { Value: '103', Text: '103 Isenção do ICMS no Simples Nacional para faixa de receita bruta' },
                            { Value: '201', Text: '201 Tributada pelo Simples Nacional com permissão de crédito e com cobrança do ICMS por substituição tributária' },
                            { Value: '202', Text: '202 Tributada pelo Simples Nacional sem permissão de crédito e com cobrança do ICMS por substituição tributária' },
                            { Value: '203', Text: '203 Isenção do ICMS no Simples Nacional para faixa de receita bruta e com cobrança do ICMS por substituição tributária' },
                            { Value: '300', Text: '300 Imune' },
                            { Value: '400', Text: '400 Não tributada pelo Simples Nacional' },
                            { Value: '500', Text: '500 ICMS cobrado anteriormente por substituição tributária (substituído) ou por antecipação' },
                            { Value: '900', Text: '900 Outros' }

        ];



        Metronic.blockUI({
            boxed: true
        });

        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaentradaSaida = [{ 'Text': 'Entrada', 'Value': 'E' }, { 'Text': 'Saída', 'Value': 'S' }];
        $scope.ListaprodutoServico = [{ 'Text': 'Produto', 'Value': 'P' }, { 'Text': 'Serviço', 'Value': 'S' }];
        
        $scope.operacao = "";

        $scope.PS = function () {
            if ($scope.operacao.produtoServico == "P")
                $scope.ShowServico = false;
            else
                $scope.ShowServico = true;
        }

        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.CalculoImpostos = [];
            $scope.LoadOperacaoporId(0);

        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            if (!$scope.ModoConsulta)
                $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadOperacaoporId(item.id);

        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadOperacaoporId(item.id);

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
            $scope.LoadOperacao();
        }
		
        $scope.Excel = function (msg) {
            window.location = '../../MT/Excel';
        }

        $scope.Salvar = function () {
		        
            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;


            var postdata = $scope.operacao;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };
            postdata.CalculoImposto = $scope.CalculoImpostos;

            $http.post("../../MT/Salvar", postdata)
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
            $location.hash('DivTotal');
        };

        $scope.LoadOperacao = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../MT/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.LoadOperacao();

        $scope.LoadOperacaoporId = function (id) {
            $http.get("../../MT/ObterPorId/" + id)
            .success(function (data) {
                $scope.operacao = data;
                $scope.CalculoImpostos = data.CalculoImpostos;
                $http.get("../../MT/NovoCalculoImposto/0")
                .success(function (data)
                {
                    $scope.CalculoImposto = [];
                    $scope.CalculoImposto= data.ci;
                    $scope.PS();
                    $scope.showGrid = false;
                    $scope.showManut = true;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
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
                $scope.LoadOperacao();
            }, function () {
            });
        }


        //Lista TipoRPS
        $scope.LoadListaTipoRPS = function () {
            $http.post("../../MT/ListaTipoRPS")
                .success(function (data) {
                    $scope.ListaTipoRPS = data;
                });
        }

        //Lista SituacaoTributaria
        $scope.LoadListaSituacaoTributaria = function () {
            $http.post("../../MT/ListaSituacaoTributaria")
                .success(function (data) {
                    $scope.ListaSituacaoTributaria = data;
                });
        }



        //Lista Impostos
        $scope.LoadListaImpostos = function () {
            $http.post("../../MT/ListaImposto")
                .success(function (data) {
                    $scope.ListaImpostos = data;
                    ListaImpostos = data;
                });
        }


        $scope.LoadListaCFOP = function () {
            $http.post("../../MT/ObterCFOP")
                .success(function (data) {
                    $scope.ListaCFOP = data;
                });
        }

        $scope.LoadListaCST = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../MT/ObterCST", "{impostoid : " + $scope.CalculoImposto.imposto_id + "}")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaCST = data;
                });
        }

        $scope.fcAdiciona = function (obj) {
            $scope.ShowDivValidacao = false;

            if (angular.isUndefined($scope.CalculoImposto.retido) || $scope.CalculoImposto.retido == null) {
                obj.retido = "false";
            }

            //Verifica se o imposto foi selecionado
            if (angular.isUndefined($scope.CalculoImposto.imposto_id) || $scope.CalculoImposto.imposto_id == null) {
                $scope.MsgDivValidacao = "Informe o Imposto.";
                $scope.ShowDivValidacao = true;
                return;
            }

            //Verifica se a alíquota foi informada
            if ($scope.CalculoImposto.aliquota == null) {
                $scope.MsgDivValidacao = "Informe a Alíquota.";
                $scope.ShowDivValidacao = true;
                return;
            }

            //Verifica se o arrecadador foi informado
            if ($scope.CalculoImposto.arrecadador == null || $scope.CalculoImposto.arrecadador == "") {
                $scope.MsgDivValidacao = "Informe o Arrecadador.";
                $scope.ShowDivValidacao = true;
                return;
            }

            //Verifica se o imposto já foi incluído
            if (impostoExiste()) {
                $scope.MsgDivValidacao = "Imposto já informado";
                $scope.ShowDivValidacao = true;
                return;
            }

            $scope.objAux = {};
            angular.copy(obj, $scope.objAux);
            if ($scope.CalculoImpostos == null)
                $scope.CalculoImpostos = [];

            $scope.CalculoImpostos.push($scope.objAux);
            
        }

        function impostoExiste() {
            var existe = false;
            angular.forEach($scope.CalculoImpostos, function (value) {
                if (value.imposto_id == $scope.CalculoImpostos.imposto_id) {
                    existe = true;
                }
            });
            return existe;
        }

        $scope.fcExcluir = function (id) {
            $scope.CalculoImpostos.splice(id, 1);
        }

        $scope.fcCST = function () {
            $scope.LoadListaCST();
        }

        $scope.LoadListaCFOP();
        $scope.LoadListaImpostos();
        $scope.LoadListaTipoRPS();
        $scope.LoadListaSituacaoTributaria();

    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../MT/Excluir", { id: $scope.item.id })
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


