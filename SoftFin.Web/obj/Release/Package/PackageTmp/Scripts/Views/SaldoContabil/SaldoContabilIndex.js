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

            element.datetimepicker({
                dateFormat: 'MM/yyyy',
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
        //Variaveis
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
 
        $scope.ListSituacao = [{ Value: "0", Text: "Em Aberto" }, { Value: "1", Text: "Cancelado" }, { Value: "9", Text: "Fechado" }];
        //Funcões Staticas
        $scope.MsgError = function (msg) {
            $scope.msgNOK = "Erro ao executar a tarefa";
            $scope.errors = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        };
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        //Funcões Botôes
        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../SaldoContabil/ObterTodos/")
                .success(function (data) {
                    $scope.listSaldoContabil = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        };
        

        $scope.New = function () {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        };
        $scope.Back = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
            $scope.msgNOK = '';
            $scope.msgOK = '';
            if (!$scope.ObterTodos)
                $scope.Pesquisa();
        };
        $scope.Edit = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.Id);
        };
        $scope.Detail = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.Id);
        };



        $scope.Salvar = function () {


            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";


            $http.post("../../SaldoContabil/Salvar", $scope.entidade)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.MsgSucess(data.DSMessage);
                        $scope.ObterTodos();

                    } else {
                        debugger
                        $scope.MsgError(data.Errors);

                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                            $scope.msgSalvar = "Salvar";

                        }, 8000);



                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";

                    $scope.msgSalvar = "Salvar";

                });
        };



        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../SaldoContabil/ObterPorId", { id: value })
                .success(function (data) {
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    Metronic.unblockUI();
                    $scope.entidade = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
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
                $scope.ObterTodos();
            }, function () {
            });
        };

        $scope.SeekContaContabil = function (item) {
            
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalContaContabil.html',
                controller: ControllerContaContabil,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: 0 }); // pass name to Dialog
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                
                item.CodigoConta = value.codigo;
                item.DescricaoConta = value.descricao;
            }, function () {

            });
        };

        $scope.SeekCentroCusto = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalCentroCusto.html',
                controller: ControllerCentroCusto,
                resolve: {} // empty storage
            };
           

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: 0 }); // pass name to Dialog
            };
            
            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.entidade.CodigoCentroCusto = value.Codigo;
                $scope.entidade.DescricaoCentroCusto = value.unidade;
            }, function () {
                
            });
        };

        $scope.ExcluirItem = function (item) {
            item.deleted = true;
        };

        $scope.FiltroExcluir = function (item) {
            return !item.deleted;
        };

        $scope.NovoItem = function () {
            $scope.entidade.SaldoContabilDetalhe.push({ id: 0, deleted: false });
        },

            $scope.ObterTodos();





}]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    console.log(item.item);

    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../SaldoContabil/Excluir", { obj: $scope.item })
            .success(function (data) {
                Metronic.unblockUI();
                if (data.CDStatus == "OK") {
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

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};




var ControllerCentroCusto = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.Locate = "";
    $scope.List = [];
    $scope.reverse = false;


    $scope.ObterTodos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../SaldoContabil/ObterTodosCentroCustos/")
            .success(function (data) {
                $scope.List = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
    };

    $scope.ObterTodos();

    $scope.Order = function (keyname) {
        $scope.sortKey = keyname;
        $scope.reverse = !$scope.reverse;
    };

    Metronic.blockUI({
        boxed: true
    });
    $scope.Select = function (value) {
        $modalInstance.close(value);
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    Metronic.unblockUI();
}

var ControllerContaContabil = function ($scope, $modalInstance, $modal, $http, $timeout, item) {
    $scope.Locate = "";
    $scope.List = [];
    $scope.reverse = false;


    $scope.ObterTodos = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../SaldoContabil/ObterTodosContaContabil/")
            .success(function (data) {
                $scope.List = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
    };

    $scope.ObterTodos();

    $scope.Order = function (keyname) {
        $scope.sortKey = keyname;
        $scope.reverse = !$scope.reverse;
    };

    Metronic.blockUI({
        boxed: true
    });
    $scope.Select = function (value) {
        $modalInstance.close(value);
    }
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    Metronic.unblockUI();
}
