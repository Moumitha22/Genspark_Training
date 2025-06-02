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

        [HttpPost("transfer")]
        public async Task<ActionResult<Transaction>> Transfer([FromBody] TransactionTransferRequestDto dto)
        {
            try
            {
                var result = await _transactionService.Transfer(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-account/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByAccountNumber(string accountNumber)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByAccountNumber(accountNumber);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
