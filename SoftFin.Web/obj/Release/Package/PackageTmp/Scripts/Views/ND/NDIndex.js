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
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {


        $scope.EditAccess = false;
        $http.post("../../ND/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

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
        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ND/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
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
            $scope.ObterTodos();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }
        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.id);
        }
        $scope.Salvar = function () {


            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalSalvar.html',
                controller: ControllerSalvar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.entidade }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 18000);
            }, function () {
            });

        }


        $scope.Receber = function (item) {


            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalReceber.html',
                controller: ControllerReceber,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 18000);
            }, function () {
            });

        }
        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ND/ObterPorId", { id: value })
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
                $scope.ObterTodos();
            }, function () {
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
                        $scope.entidade.cliente_id = value.id;
                        $scope.entidade.cliente_nome = value.nome;


                    }, function () {
                    });

                });
        }

        $scope.ObterBanco = function () {

            $http.post("../../ND/ObterBanco")
                .success(function (data) {
                    $scope.ListaBanco = data;
                });
        };

        $scope.ObterPlanoConta = function () {

            $http.post("../../ND/ObterPlanoConta")
                .success(function (data) {
                    $scope.ListaPlanoConta = data;
                });
        };

        $scope.ObterPlanoConta();
        $scope.ObterBanco();
        $scope.ObterTodos();
    }]);


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




var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../ND/Excluir", { obj: $scope.item })
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


var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {



    $scope.entidade = item.item;

    $scope.ok = function () {
        $http.post("../../ND/Salvar", { obj: $scope.entidade, receber: false })
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


var ControllerReceber = function ($scope, $modalInstance, $modal, $http, $timeout, item) {



    $scope.entidade = item.item;

    $scope.ok = function () {
        $http.post("../../ND/Salvar", { obj: $scope.entidade, receber: true })
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