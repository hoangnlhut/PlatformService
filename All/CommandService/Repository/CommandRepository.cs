using CommandService.Data;
using CommandService.Models;

namespace CommandService.Repository
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Platform related methods
        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.Platforms.Add(plat);
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }
        #endregion

        #region Command related methods
        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
        }

        public Command? GetCommand(int platformId, int commandId)
        {
            return _context.Commands.FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }
        #endregion

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
