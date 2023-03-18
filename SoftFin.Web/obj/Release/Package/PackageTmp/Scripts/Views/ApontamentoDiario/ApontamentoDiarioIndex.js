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
    };
});

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../ApontamentoDiario/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        //Variaveis
        $scope.locate = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.modeDetail = false;
        //Funcões Staticas
        $scope.MsgError = function (msg, errors) {
            $scope.msgNOK = msg;
            $scope.msgOK = "";
            
            $scope.errors = errors;
        };

        $scope.MsgSucess = function (msg) {
            $scope.msgOK = msg;
            $scope.msgNOK = "";
        };

        $scope.Order = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        $scope.Filter = {
            'Data': moment().format()
        };

        //Funcões Botôes
        $scope.Search = function () {
            $scope.ObterTodos();
        };

        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ApontamentoDiario/ObterTodos/", $scope.Filter)
                .success(function (data) {
                    $scope.listApontamentoDiarios = data;
                    
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        };


        $scope.New = function () {
            $scope.modeDetail = false;
            $scope.ObterPorId(0);
            
        };

        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ApontamentoDiario/ObterPorId", { id: value })
                .success(function (data) {
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    Metronic.unblockUI();
                    $scope.entidade = data;
                    $scope.ChangeProjeto($scope.entidade.Projeto_id);
                    
                }).error(function (data) {

                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });
        };

        $scope.Back = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.modeDetail = false;
            $scope.msgNOK = '';
            $scope.msgOK = '';
            $scope.ObterTodos();
        };

        $scope.Edit = function (item) {
            $scope.modeDetail = false;
            $scope.ObterPorId(item.id);
        };
        $scope.Detail = function (item) {
            $scope.modeDetail = true;
            $scope.ObterPorId(item.id);
        };
        
    
        $scope.Save = function (item) {
            $scope.Salvar();
        };
        

        $scope.Salvar = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../ApontamentoDiario/Salvar", $scope.entidade)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.MsgSucess(data.DSMessage);
                    } else {
                        $scope.MsgError(data.DSMessage, data.Errors);
                    }
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema Indisponivel");
                });
        };

        $scope.Delete = function (item) {
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
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Search();
            }, function () {
            });
        };

        $scope.LoadProjetos = function () {
            $http.post("../../ApontamentoDiario/ObterProjetos/")
                .success(function (data) {
                    $scope.listProjetos = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        };

        $scope.LoadProjetos();

        

        //TODO: Carrega Demais

        $scope.LoadAtividades = function (id) {
            if (id === null) {
                $scope.listAtividades = [];
            }
            else {
                $http.get("../../ApontamentoDiario/ObterAtividades/?projeto_id=" + id)
                    .success(function (data) {
                        $scope.listAtividades = data;
                    }).error(function (data) {
                        $scope.MsgError("Sistema indisponivel");
                    });
            }
        };

        $scope.ChangeProjeto = function (id) {
            $scope.LoadAtividades(id);
        };
        $scope.Search();
    }


]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;

    $scope.OK = function () {
        Metronic.blockUI({
            boxed: true
        });
        $http.post("../../ApontamentoDiario/Excluir", { obj: $scope.entidade })
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus === "OK") {
                    $modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                    $scope.Erros = data.Erros;
                }

            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
