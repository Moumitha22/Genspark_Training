using BankManagementSystem.Interfaces;
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<Transaction>> Deposit([FromBody] TransactionAddRequestDto dto)
        {
            try
            {
                var result = await _transactionService.Deposit(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Transaction>> Withdraw([FromBody] TransactionAddRequestDto dto)
        {
            try
            {
                var result = await _transactionService.Withdraw(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check-balance/{accountId}")]
        public async Task<ActionResult<decimal>> CheckBalance(int accountId)
        {
            try
            {
                var balance = await _transactionService.CheckBalance(accountId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
