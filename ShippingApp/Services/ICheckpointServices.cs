using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface ICheckpointServices
    {
        public ResponseModel addCheckpoint(AddCheckpointModel checkpoint);
        public ResponseModel getCheckpoints(Guid checkpointId, string checkpointName);
        public ResponseModel getDistance(Guid cp1Id, Guid cp2Id);
        public ResponseModel AddGrandParent(AddCheckpointModel gp);

	}
}
