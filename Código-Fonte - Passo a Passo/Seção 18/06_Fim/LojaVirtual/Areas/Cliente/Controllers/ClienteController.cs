﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Areas.Cliente.Controllers
{
    public class ClienteController : Controller
    {
        private LoginCliente _loginCliente;
        private ClienteRepository _clienteRepository;

        public ClienteController(LoginCliente loginCliente, ClienteRepository clienteRepository)
        {
            _loginCliente = loginCliente;
            _clienteRepository = clienteRepository;
        }

        [ClienteAutorizacao]
        public IActionResult Index()
        {
            return View();
        }

        [ClienteAutorizacao]
        [HttpGet]
        public IActionResult Atualizar()
        {
            Models.Cliente cliente = _clienteRepository.ObterCliente(_loginCliente.GetCliente().Id);
            
            return View(cliente);
        }

        [ClienteAutorizacao]
        [HttpPost]
        public IActionResult Atualizar(Models.Cliente cliente)
        {
            ModelState.Remove("Senha");
            ModelState.Remove("ConfirmacaoSenha");

            if (ModelState.IsValid)
            {
                cliente.Senha = _loginCliente.GetCliente().Senha;
                _clienteRepository.Atualizar(cliente);

                _loginCliente.Login(cliente);

                TempData["MSG_S"] = Mensagem.MSG_S001;

                return RedirectToAction(nameof(Index));
                
            }
            return View();
        }


        [ClienteAutorizacao]
        [HttpGet]
        public IActionResult AtualizarSenha()
        {
            return View();
        }

        [ClienteAutorizacao]
        [HttpPost]
        public IActionResult AtualizarSenha(Models.Cliente cliente)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Nascimento");
            ModelState.Remove("Sexo");
            ModelState.Remove("CPF");
            ModelState.Remove("Telefone");
            ModelState.Remove("Email");
            ModelState.Remove("CEP");
            ModelState.Remove("Estado");
            ModelState.Remove("Cidade");
            ModelState.Remove("Bairro");
            ModelState.Remove("Endereco");
            ModelState.Remove("Complemento");
            ModelState.Remove("Numero");

            if (ModelState.IsValid)
            {
                Models.Cliente clienteDB = _clienteRepository.ObterCliente(_loginCliente.GetCliente().Id);
                clienteDB.Senha = cliente.Senha;
                _clienteRepository.Atualizar(clienteDB);

                _loginCliente.Login(clienteDB);

                TempData["MSG_S"] = Mensagem.MSG_S001;

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

    }
}