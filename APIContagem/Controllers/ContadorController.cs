using Microsoft.AspNetCore.Mvc;
using APIContagem.Models;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private static readonly Contador _CONTADOR = new Contador();
    private readonly ILogger<ContadorController> _logger;
    private readonly IConfiguration _configuration;

    public ContadorController(ILogger<ContadorController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public ResultadoContador Get()
    {
        int valorAtualContador;

        lock (_CONTADOR)
        {
            _CONTADOR.Incrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }

        _logger.LogInformation($"Contador - Valor atual: {valorAtualContador}");

        var saudacao = _configuration["Mensagens:Saudacao"];
        _logger.LogInformation($"Mensagens:Saudacao = {saudacao}");

        var aviso = _configuration["Mensagens:Aviso"];
        _logger.LogInformation($"Mensagens:Aviso = {aviso}");

        return new()
        {
            ValorAtual = valorAtualContador,
            Producer = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Framework = _CONTADOR.Framework,
            Mensagem = _configuration["Mensagens:Contador"],
            Saudacao = saudacao,
            Aviso = aviso
        };
    }
}