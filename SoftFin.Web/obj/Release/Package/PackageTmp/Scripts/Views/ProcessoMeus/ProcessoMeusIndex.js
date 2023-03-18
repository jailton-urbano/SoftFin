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
        return input === true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$sce', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $sce, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.CodigoProcesso = "";
        $scope.procurar = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.ModoConsulta = false;
        $scope.showPesquisa = true;
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
        $scope.ObterUrl = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.get("../../ProcessoAndamento/GetUrlProcesso")
                .success(function (data) {
                    $scope.url = data;
                    $scope.urliframe = $sce.trustAsResourceUrl($scope.url + "/Aguarde/Index");
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.ObterUrl();
        $scope.Detalhar = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: '../../page/ProcessoHistorico/consulta.html',
                controller: ControllerDetalhar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodos();
            }, function () {
            });
        }
        //Funcões Staticas
        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        //Funcões Botôes
        $scope.Iniciar = function (item) {
            
            $scope.urliframe = $sce.trustAsResourceUrl($scope.url + "/Dinamico/" + item.Action + "?CodigoProcesso=" + item.CodigoProcesso + "&CodigoAtividade=" + item.CodigoAtividade + "&CodigoAtividadeAtual=" + item.CodigoAtividadeAtual + "&CodigoUsuario=" + item.CodigoUsuario + "&CodigoEmpresa=" + item.CodigoEmpresa + "&NumeroProtocolo=" + item.NumeroProtocolo + "&CodigoAtividadeExecucaoAtual=" + item.CodigoAtividadeExecucaoAtual + "&CodigoProcessoAtual=" + item.CodigoProcessoAtual + "");
            $scope.showPesquisa = false;
        }

        $scope.voltar = function (item) {
            $scope.ObterTodos();
            $scope.urliframe = $sce.trustAsResourceUrl($scope.url + "Aguarde/Index");
            $scope.showPesquisa = true;
        }
        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ProcessoMeus/Obter/", { codigoProcesso: $scope.CodigoProcesso })
                .success(function (data) {
                    $scope.lista = data.Objs;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
            if (!$scope.ObterTodos)
                $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }


        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ProcessoMeus/ObterPorId", { id: value })
                .success(function (data) {
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    Metronic.unblockUI();
                    $scope.entidade = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        $scope.Cancelar = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalCancelar.html',
                controller: ControllerCancelar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodos();
            }, function () {
            });
        }


        $scope.LoadProcessos = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../ProcessoMeus/ObterProcessos")
                .success(function (data) {
                    Metronic.unblockUI();
                    $scope.ListaProcessos = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });

        };

        $scope.LoadProcessos();
     }]);







var ControllerCancelar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../ProcessoMeus/Excluir", { obj: $scope.item })
            .success(function (data) {
                if (data.CDStatus === "OK") {
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


var ControllerDetalhar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;
    $scope.LoadDetalhe = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../ProcessoAndamento/Detalhar", { 'codigoProcessoAtual': $scope.item.CodigoProcessoAtual })
            .success(function (data) {
                Metronic.unblockUI();
                $scope.Lista = data;
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

    };

    $scope.LoadDetalhe();

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}