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

app.filter('TipoConta', function () {
    return function (input) {
        if (input === 0)
            return "Conta Corrente";
        if (input === 1)
            return "Cartão Crédito";
        if (input === 2)
            return "Aplicação Financeira";
        if (input === 3)
            return "Caixinha";
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {    
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

        $scope.EditAccess = false;
        $http.post("../../BC/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        Metronic.blockUI({
            boxed: true
        });


        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ListaTipoConta = [{ Value: 0, Text: "Conta Corrente" }, { Value: 1, Text: "Cartão Crédito" }, { Value: 2, Text: "Aplicação Financeira" }, { Value: 3, Text: "Caixinha" }];
        $scope.banco = "";


        $scope.LoadListaBanco = function () {
            $http.post("../../CG/ObterBancoAplicacao")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadListaBanco();


        $scope.Novo = function () {
            $scope.ModoConsulta = false;   
           $scope.LoadbancoporId(0);
        }



        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            if (!$scope.ModoConsulta)
                $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadbancoporId(item.id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadbancoporId(item.id);
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
            $scope.Loadbanco();
        }
		
        $scope.Excel = function (msg) {
            window.location = '../../BC/Excel';
        }

        $scope.Salvar = function () {
		        
            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde";
            $scope.btnSalvar = false;


            var postdata = $scope.banco;//, notafiscal: $scope.nf, documentoPagarMestre: null, documentoPagarDetalhes: null };

            $http.post("../../BC/Salvar", postdata)
            .success(function (data) {
                Metronic.unblockUI();

                if (data.CDStatus == "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;
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

        $scope.Loadbanco = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../BC/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.Loadbanco();

        $scope.LoadbancoporId = function (id) {
            $http.get("../../BC/ObterPorId/" + id)
            .success(function (data) {
                $scope.banco = data;
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
                $scope.Loadbanco();
            }, function () {
            });
        }


    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../BC/Excluir", { id: $scope.item.id })
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


