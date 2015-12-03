using System;
using Raven.Client;
using log4net;

namespace FriendsOfDT.Tasks {
    public abstract class BackgroundTask {
        protected ILog log = LogManager.GetLogger(typeof(BackgroundTask));
        protected IDocumentSession documentSession;

        protected virtual void Initialize(IDocumentSession session) {
            documentSession = session;
            documentSession.Advanced.UseOptimisticConcurrency = true;
        }

        protected abstract void Execute();

        public bool Run(IDocumentSession openSession) {
            Initialize(openSession);
            try {
                Execute();
                documentSession.SaveChanges();
                TaskExecuter.StartExecuting();
                return true;
            } catch (Exception e) {
                log.Error("Error processing task:\r\n" + ToString(), e);
                return false;
            } finally {
                TaskExecuter.Discard();
            }
        }

        public override abstract string ToString();
    }
}