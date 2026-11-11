using Finance.Application.DTOs.Transfers;
using Finance.Application.UseCases.Transfers.CancelTransfer;
using Finance.Application.UseCases.Transfers.CreateTransfer;
using Finance.Application.UseCases.Transfers.GetAllTransfers;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly GetAllTransfersHandler _getAllTransfersHandler;
        private readonly CreateTransferHandler _createTransferHandler;
        private readonly CancelTransferHandler _cancelTransferHandler;

        public TransfersController(GetAllTransfersHandler getAllTransfersHandler, CreateTransferHandler createTransferHandler, CancelTransferHandler cancelTransferHandler)
        {
            _getAllTransfersHandler = getAllTransfersHandler;
            _createTransferHandler = createTransferHandler;
            _cancelTransferHandler = cancelTransferHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllTransfersHandler.HandleAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransferRequest request)
        {
            var result = await _createTransferHandler.HandleAsync(request);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _cancelTransferHandler.HandleAsync(id);

            return NoContent();
        }
    }
}
