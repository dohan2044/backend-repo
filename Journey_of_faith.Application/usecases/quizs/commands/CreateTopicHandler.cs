using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class CreateTopicHandler : IRequestHandler<CreateTopicCommand, int>
    {
        private readonly IExamRepository examRepository;
        private readonly IExamQueries examQueries;
        public CreateTopicHandler(IExamRepository examRepository, IExamQueries examQueries)
        {
            this.examRepository = examRepository;
            this.examQueries = examQueries;
        }


        public async Task<int> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
        {
            var topic = new Topic(command.TopicName, command.QuizCount);

            await CheckNameExists(topic.TopicName, cancellationToken);

            return await examRepository.CreateTopicAsync(topic);

        }

        private async Task<bool> CheckNameExists(string name, CancellationToken cancellationToken)
        {
            if(await examQueries.TopicNameExistsAsync(name, cancellationToken))
            {
                throw new ConfictException("Tên chủ đề đã tồn tại");
            }

            return true;
        }
    }
}
