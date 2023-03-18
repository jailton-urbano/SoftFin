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
                ngModelCtrl.$setViewValue(moment(e.date).format());
                scope.$apply();
                $(this).datetimepicker('hide');
            });
        }
    };
});

app.filter('YesNo', function () {
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
        $scope.ListYesNo = [{ Value: true, Text: "Sim" }, { Value: false, Text: "Não" }];
        $scope.ListTipo = [{ Value: "G", Text: "Grupo" }, { Value: "P", Text: "Sub-grupo" }, { Value: "A", Text: "Analitica" }, { Value: "S", Text: "Sintética" }];
        $scope.ListCategoriaGeral = [{ Value: "AN", Text: "Ativo Normal" }, { Value: "AR", Text: "Ativo Redutora" }, { Value: "PN", Text: "Passivo Normal" }, { Value: "PR", Text: "Passivo Redutora" }, { Value: "PL", Text: "Patrimonio Liquido Normal" }, { Value: "LR", Text: "Patrimonio Liquido Redutora" }, { Value: "RS", Text: "Resultado Normal" }, { Value: "RR", Text: "Resultado Redutora" }];
        $scope.ListSubCategoria = [{ Value: "ROB", Text: "Receita Operacional Bruta" }, { Value: "DRB", Text: "Deduções Receita Bruta" }, { Value: "CDO", Text: "Custos da operação/vendas" }, { Value: "DOP", Text: "Despesas da operação" }, { Value: "DFL", Text: "Despesas Financeiras Liquidas" }, { Value: "ORD", Text: "Outras Receitas e despesas não operacionais" }];
        $scope.ListPublicacao = [{ Value: "DC", Text: "Demonstrativos Contabeis (Balanço Patrimonial, DRE, etc)" }, { Value: "BL", Text: "Balancete (detalhado)" }, { Value: "AB", Text: "Ambos" }];

        //Funcões Staticas

        $scope.EditAccess = false;
        $http.post("../../ContaC/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });

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

            $http.post("../../ContaC/ObterTodos/")
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
            $scope.LoadCC();
            $scope.Pesquisa();
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


            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";


            $http.post("../../ContaC/Salvar", $scope.entidade)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        $location.hash('showOKTop');
                        $scope.msgSalvar = "Salvar";
                        $scope.ObterTodos();

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

                        }, 8000);



                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";

                    $scope.msgSalvar = "Salvar";

                });
            

        }
        
        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ContaC/ObterPorId", { id: value })
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

        $scope.LoadCC = function () {
            $http.post("../../CG/ObterContaContabilPorCodigo/")
                .success(function (data) {
                    $scope.ListaContaContabil = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        

        $scope.LoadObterPlanoContas = function () {
            $http.post("../../CG/ObterPlanoContasDebito/")
                .success(function (data) {
                    $scope.ListaPlanoContas = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }

        $scope.Upload = function () {
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
                return angular.copy({ item: 1 }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                //console.log("Modal OK");
            }, function () {
                //on cancel button press
                //console.log("Modal Closed");
            });
        };


        $scope.LoadCC();
        $scope.LoadObterPlanoContas();

        $scope.ObterTodos();
    }]);







var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        $http.post("../../ContaC/Excluir", { obj: $scope.item })
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


var ControllerUpload = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item;
    $scope.arquivo = "";

    $scope.someMsg = function () {
        $scope.showNOK = false;
        $scope.showOK = false;
    };

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        var f = document.getElementById('newPhotos').files[0];
        var fd = new FormData();
        fd.append("file", f);
       
        $http.post("../../../ContaC/UploadECD", fd, {
            withCredentials: true,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus === "OK") {
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
                Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}



