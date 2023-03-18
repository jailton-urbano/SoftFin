var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination', 'blockUI']);
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
        return input == true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', 'blockUI','$sce',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal, blockUI, $sce) {
        blockUI.start("Aguarde...");
        //Variaveis
        $scope.procurar = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.ModoConsulta = false;
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

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
            blockUI.start("Aguarde...");

            $http.post("../../PR/ObterTodosProcessos/")
                .success(function (data) {
                    blockUI.stop();
                    $scope.ListaProcessos = data;
                    
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                    
                });
        }
        $scope.ObterTodosCampos = function () {
            blockUI.start("Aguarde...");

            $http.post("../../PR/ObterTodosProcessos/")
                .success(function (data) {
                    blockUI.stop();
                    $scope.tabela.TabelaCampos = data;

                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");

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
            $scope.ObterPorId(item.Id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.Id);
        }

        $scope.Salvar = function () {
            blockUI.start("Aguarde...");

            $http.post("../../PR/SalvarProcesso", { "obj": $scope.processo })
                .success(function (data) {
                    blockUI.stop();

                    if (data.CDStatus == "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        
                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
                        
                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                        }, 8000);



                }).error(function (data) {
                    blockUI.stop();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });
        }

        $scope.ObterPorId = function (value) {
            blockUI.start("Aguarde...");

            $http.post("../../PR/ObterProcessosPorId", { id: value })
                .success(function (data) {
                    
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    blockUI.stop();
                    $scope.processo = data;
                    $scope.ObterAtividades();
                    $scope.ObterPlanos();
                 //   $scope.MostraIframe();
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        
        $scope.ObterAtividades = function () {
            blockUI.start("Aguarde...");

            $http.post("../../PR/ObterTodosAtividades", { 'idProcesso': $scope.processo.Id})
                .success(function (data) {

                    $scope.showGrid = false;
                    $scope.showManut = true;
                    blockUI.stop();
                    $scope.ListaAtividade = data;
                    //   $scope.MostraIframe();
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                });
        }


        $scope.ObterPlanos = function () {
            blockUI.start("Aguarde...");

            $http.post("../../PR/ObterTodosPlanos", { 'idProcesso': $scope.processo.Id })
                .success(function (data) {

                    $scope.showGrid = false;
                    $scope.showManut = true;
                    blockUI.stop();
                    $scope.ListaPlano = data;
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        //$scope.LoadTabelas = function () {
        //    $http.post("../../PR/ObterTabelas")
        //        .success(function (data) {
        //            $scope.ListaTabelas = data;
        //        });
        //};
        $scope.showiframe = false;
        //$scope.MostraIframe = function () {
        //    $scope.showiframe = true;
        //    $scope.urliframe = $sce.trustAsResourceUrl("../../Dinamico/ShowTable?tabela=" + $scope.tabela.Id);
        //}

        $scope.Excluir = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirProcesso.html',
                controller: ControllerExcluirProcesso ,
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

        $scope.ManutAtividade = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalAtividade.html',
                controller: ControllerManutAtividade,
                resolve: {} // empty storage
            };

            if (item == null) {
                $scope.opts.resolve.item = function () {
                    return angular.copy({
                        item: {
                            "Id": 0,
                            "Ativo": true,
                            "Descricao": "",
                            "IdProcesso": $scope.processo.Id

                        }
                    }); 
                }
            }
            else {
                $scope.opts.resolve.item = function () {
                    item.IdProcesso = $scope.processo.Id;
                    return angular.copy({ item: item }); 
                }
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterAtividades();
            }, function () {
            });
        }


        $scope.ManutPlano = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalPlano.html',
                controller: ControllerManutPlano,
                resolve: {} 
            };

            if (item == null) {
                $scope.opts.resolve.item = function () {
                    return angular.copy({
                        item: {
                            "Id": 0,
                            "CondicaoEntrada": "",
                            "ProcessoId": $scope.processo.Id

                        }
                    });
                }
            }
            else {
                $scope.opts.resolve.item = function () {
                    item.IdProcesso = $scope.processo.Id;
                    return angular.copy({ item: item });
                }
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterPlanos();
            }, function () {
            });
        }

        $scope.ExcluirPlano = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirPlano.html',
                controller: ControllerExcluirPlano,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterPlanos();
            }, function () {
            });
        }

        $scope.ExcluirAtividade = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirAtividade.html',
                controller: ControllerExcluirAtividade,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterAtividades();
            }, function () {
            });
        }
        $scope.ObterTodos();




        $timeout(
            function () {
                blockUI.stop();

            }, 1000);

    }]);


var ControllerManutAtividade = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {

    $scope.item = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.LoadAtividadeTipo = function () {
        $http.post("../../PR/ObterAtividadeTipo")
            .success(function (data) {
                $scope.ListaTipoAtividade = data;
            });
    };
    $scope.LoadAtividadeTipo();

    $scope.LoadListaFuncao = function () {
        $http.post("../../PR/ObterFuncao", { 'idAtividade': $scope.item.Id })
            .success(function (data) {
                $scope.ListaFuncao = data;
            });
    };
    $scope.LoadListaFuncao();


    $scope.LoadListaVisao = function () {
        $http.post("../../PR/ObterListaVisao", { 'idAtividade': $scope.item.Id })
            .success(function (data) {
                $scope.ListaVisao = data;
            });
    };
    $scope.LoadListaVisao();

    //$scope.LoadObterTipoCampos = function () {
    //    $http.post("../../PR/ObterTipoCampos")
    //        .success(function (data) {
    //            $scope.ListaTipoCampos = data;
    //        });
    //};
    //$scope.LoadObterTipoCampos();


    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/SalvarAtividade", { 'obj': $scope.item, 'funcoes': $scope.ListaFuncao, 'visaos': $scope.ListaVisao })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


var ControllerManutPlano = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {

    $scope.item = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.LoadObterTodosAtividadesCombo = function () {
        $http.post("../../PR/ObterTodosAtividadesCombo", { 'idProcesso': $scope.item.ProcessoId })
            .success(function (data) {
                $scope.ListaAtividades = data;
            });
    };
    $scope.LoadObterTodosAtividadesCombo();






    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/SalvarAtividadePlanos", { 'obj': $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


var ControllerExcluirProcesso = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/ExcluirProcessos", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerExcluirAtividade = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/ExcluirAtividade", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}


var ControllerExcluirPlano = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/ExcluirAtividadePlanos", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
var ControllerExcluirCampo = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {

    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/ExcluirCampo", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerEditarCampo = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI,item) {

    $scope.item = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.LoadTabelas = function () {
        $http.post("../../PR/ObterTabelas")
            .success(function (data) {
                $scope.ListaTabelas = data;
            });
    };
    $scope.LoadTabelas();
    $scope.LoadObterTipoCampos = function () {
        $http.post("../../PR/ObterTipoCampos")
            .success(function (data) {
                $scope.ListaTipoCampos = data;
            });
    };
    $scope.LoadObterTipoCampos();

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../PR/SalvarCampo", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

