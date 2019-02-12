using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arma3BE.Client.Modules.BEServerModule.Tests
{
    [TestClass]
    public class BEServiceLogicTests
    {
        [TestMethod]
        public void Player_Log_Test()
        {
            var serverGuid = Guid.NewGuid();

            var ev = new FakeEventAggregator();

            var logic = new BELogic(ev);

            BECommand command = null;

            logic.ServerUpdateHandler += (s, e) =>
            {
                command = e.Command;
            };


            ev.GetEvent<BEMessageEvent<BEPlayerLogMessage>>()
               .Publish(new BEPlayerLogMessage(new LogMessage(), serverGuid));


            Assert.IsNotNull(command);
            Assert.AreEqual(CommandType.Players, command.CommandType);

            GC.KeepAlive(logic);
        }

        [TestMethod]
        public void Admin_Test()
        {
            var serverGuid = Guid.NewGuid();

            var ev = new FakeEventAggregator();

            var logic = new BELogic(ev);

            BECommand command = null;

            logic.ServerUpdateHandler += (s, e) =>
            {
                command = e.Command;
            };


            ev.GetEvent<BEMessageEvent<BEAdminLogMessage>>()
               .Publish(new BEAdminLogMessage(new LogMessage(), serverGuid));


            Assert.IsNotNull(command);
            Assert.AreEqual(CommandType.Admins, command.CommandType);

            GC.KeepAlive(logic);
        }


        [TestMethod]
        public void Connect_Test()
        {
            var serverGuid = Guid.NewGuid();

            var ev = new FakeEventAggregator();

            var logic = new BELogic(ev);

            var commands = new List<BECommand>();
            var immediateCommands = new List<BECommand>();

            logic.ServerUpdateHandler += (s, e) =>
            {
                commands.Add(e.Command);
            };

            logic.ServerImmediateUpdateHandler += (s, e) =>
            {
                immediateCommands.Add(e.Command);
            };


            ev.GetEvent<ConnectServerEvent>()
               .Publish(new ServerInfoDto() { Id = serverGuid });

            Assert.IsNotNull(immediateCommands);
            Assert.IsNotNull(immediateCommands.FirstOrDefault(x => x.CommandType == CommandType.Players && x.ServerId == serverGuid));

            Assert.IsNotNull(commands);
            Assert.IsNotNull(commands.FirstOrDefault(x => x.CommandType == CommandType.Bans && x.ServerId == serverGuid));
            Assert.IsNotNull(commands.FirstOrDefault(x => x.CommandType == CommandType.Admins && x.ServerId == serverGuid));
            Assert.IsNotNull(commands.FirstOrDefault(x => x.CommandType == CommandType.Missions && x.ServerId == serverGuid));

            GC.KeepAlive(logic);
        }


        [TestMethod]
        public void Player_Ban_Test()
        {
            var serverGuid = Guid.NewGuid();

            var ev = new FakeEventAggregator();

            var logic = new BELogic(ev);

            var commands = new List<BECommand>();

            logic.ServerUpdateHandler += (s, e) =>
            {
                commands.Add(e.Command);
            };


            ev.GetEvent<BEMessageEvent<BEBanLogMessage>>()
               .Publish(new BEBanLogMessage(new LogMessage(), serverGuid));

            Assert.IsNotNull(commands);
            Assert.IsNotNull(commands.FirstOrDefault(x => x.CommandType == CommandType.Bans && x.ServerId == serverGuid));
            Assert.IsNotNull(commands.FirstOrDefault(x => x.CommandType == CommandType.Players && x.ServerId == serverGuid));

            GC.KeepAlive(logic);
        }
    }

    class FakeEventAggregator : IEventAggregator
    {
        private readonly Dictionary<string, EventBase> _events = new Dictionary<string, EventBase>();

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            var evType = typeof(TEventType);
            var typeArgs = new List<Type> { evType };

            typeArgs.AddRange(GetGenericArguments(evType));

            var method = typeof(FakeEventAggregator).GetMethod(nameof(CreateEvent), BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = method.MakeGenericMethod(typeArgs.ToArray());

            return generic.Invoke(this, null) as TEventType;
        }

        private Type[] GetGenericArguments(Type source)
        {
            if (source.IsGenericType) return source.GenericTypeArguments;
            if (source.BaseType != null) return GetGenericArguments(source.BaseType);
            return null;
        }

        private EventBase CreateEvent<T, TK>() where T : PubSubEvent<TK>
        {
            var sId = typeof(T).FullName;

            if (_events.ContainsKey(sId)) return _events[sId];

            var mockedEvent = new Mock<T>();
            var cont = new ActionContainer<TK>();

            mockedEvent.Setup(x =>
            x.Subscribe(
                It.IsAny<Action<TK>>(),
                It.IsAny<ThreadOption>(),
                It.IsAny<bool>(),
                It.IsAny<Predicate<TK>>()
                ))
                .Callback<Action<TK>, ThreadOption, bool, Predicate<TK>>(
                    (a, o, b, p) =>
                    {
                        cont.Inner = a;
                    });

            mockedEvent.Setup(x => x.Publish(It.IsAny<TK>()))
                .Callback<TK>(m =>
                {
                    cont.Inner(m);
                });


            _events.Add(sId, mockedEvent.Object);

            return mockedEvent.Object;
        }


        private class ActionContainer<T>
        {
            public Action<T> Inner { get; set; }
        }
    }
}
