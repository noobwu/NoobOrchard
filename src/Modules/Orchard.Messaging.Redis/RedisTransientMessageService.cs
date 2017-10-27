//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;

namespace Orchard.Messaging.Redis
{
	public class RedisTransientMessageService
		: TransientMessageServiceBase
	{
		private readonly RedisTransientMessageFactory messageFactory;

		public RedisTransientMessageService(int retryAttempts, TimeSpan? requestTimeOut,
			RedisTransientMessageFactory messageFactory)
			: base(retryAttempts, requestTimeOut)
		{
            this.messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
		}

		public override IMessageFactory MessageFactory
		{
			get { return messageFactory; }
		}
	}

}