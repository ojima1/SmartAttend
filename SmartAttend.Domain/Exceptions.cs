using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain
{
    namespace SmartAttend.Domain.Exceptions
    {
        public abstract class DomainException : Exception
        {
            protected DomainException(string msg) : base(msg) { }
        }

        public class NotFoundException : DomainException
        {
            public NotFoundException(string msg) : base(msg) { }
        }

        public class ConflictException : DomainException
        {
            public ConflictException(string msg) : base(msg) { }
        }

        public class VpnDetectedException : DomainException
        {
            public VpnDetectedException(string msg = "VPN usage detected.") : base(msg) { }
        }

        public class LivenessException : DomainException
        {
            public LivenessException(string msg = "Liveness check failed.") : base(msg) { }
        }

        public class FaceMatchException : DomainException
        {
            public FaceMatchException(string msg = "Face does not match profile.") : base(msg) { }
        }

        public class LocationException : DomainException
        {
            public LocationException(string msg) : base(msg) { }
        }

        public class ForbiddenException : DomainException
        {
            public ForbiddenException(string msg) : base(msg) { }
        }
    }
}
