
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

app.controller('MestreCPAG', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        Metronic.blockUI({
            boxed: true
        });

        $scope.EditAccess = false;
        $http.post("../../Estoque/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

        $scope.procurar = "";
        $scope.sortKey = "";
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.showGrid = true;
        $scope.ObterTodos = function () {
            $scope.MsgDivDanger = "";
            $scope.MsgDivSuccess = "";
            $scope.ShowDivValidacao = false;

            $http.post('../../Estoque/ObterTodos')
            .success(function (data) {
                if (data.CDStatus == "OK") {
                    $scope.ListaMovto = data.objs;
                } else {
                    $scope.MsgDivDanger = data.DSMessage;
                }
                Metronic.unblockUI();
            })
            .error(function (data) {
                $scope.MsgDivDanger = 'Sistema Indisponível';
                Metronic.unblockUI();
            });

            Metronic.blockUI({
                boxed: true
            });

        }
        $scope.Lancamento = function () {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalLancamento.html',
                controller: ControllerLancamento,
                resolve: {} // empty storage
            };



            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodos();
            }, function () {
            });
        }
        $scope.Detalhe = function (item) {

            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalDetalhe.html',
                controller: ControllerDetalhe,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item.id }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {

            }, function () {
            });
        }
        $scope.ObterTodos();
    }
]);

var ControllerLancamento = function ($scope, $modalInstance, $modal, $http, $timeout) {
       
    $scope.showSalvar = true;
    //Primeiro Passo declarar variavel
    $scope.btnOK = "OK";

    $scope.entidade = { valorUnitario: 0, unidadeMedida: '', unidadeTotal: 0, quantidade: 0, valorTotal: 0.00 };
    $scope.ListaTipoMov = [{ Value: "E", Text: "Entrada" }, { Value: "S", Text: "Saida" }];
    $scope.ok = function () {
        
        //Segundo Passo Alterar Label ao clicar no botão
        $scope.btnOK = "Aguarde...";

        $http.post("../../Estoque/Salvar", $scope.entidade)
        .success(function (data) {
            //Terceiro Passo declarar variavel
            $scope.btnOK = "OK";

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
            //Terceiro Passo declarar variavel
            $scope.btnOK = "OK";

            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }
    $scope.ObterTabelas = function () {
        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;

        $http.post('../../Estoque/ListaTabelas')
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.ListaTabelas = data.objs;
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
    $scope.ObterProdutos = function () {

        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;

        if ($scope.entidade.tabelaid == null) {
            $scope.objsProdutos = [];
            return;
        }

        $http.post('../../Estoque/ListaProdutos', { idTabela: $scope.entidade.tabelaid })
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
    $scope.ObterPessoas = function () {

        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;


        $http.post('../../Estoque/ListaPessoas')
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.ListaPessoas = data.objs;
            } else {
                $scope.ListaPessoas = [];
            }
            Metronic.unblockUI();
        })
        .error(function (data) {
            $scope.ListaPessoas = [];
        });

        Metronic.blockUI({
            boxed: true
        });
    }
    $scope.CarregaPadroes = function () {
        angular.forEach($scope.objsProdutos, function (value) {
            if (value.Value == $scope.entidade.produtoid) {
                $scope.entidade.valorUnitario = value.Price;
                $scope.entidade.unidadeMedida = value.Unit;
            }
        });
    }
    $scope.SomaTotal = function () {
        $scope.entidade.valorTotal = $scope.entidade.valorUnitario * $scope.entidade.quantidade;
    }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.ObterPessoas();
    $scope.ObterTabelas();

}
var ControllerDetalhe = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.id = item.item;
    $scope.ObterDetalhe = function () {
        $scope.MsgDivDanger = "";
        $scope.MsgDivSuccess = "";
        $scope.ShowDivValidacao = false;

        $http.post('../../Estoque/Detalhe', { produtoid: $scope.id })
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.ListaDetalhe = data.objs;
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
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    $scope.ObterDetalhe();
}
