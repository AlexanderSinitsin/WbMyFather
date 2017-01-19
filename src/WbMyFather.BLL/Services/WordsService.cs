using AutoMapper;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbMyFather.BLL.Exceptions;
using WbMyFather.BLL.Services.Base;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DAL;
using WbMyFather.DAL.Entities;
using WbMyFather.DTO;
using WbMyFather.DTO.Models;
using WbMyFather.DTO.Models.Requests;

namespace WbMyFather.BLL.Services
{
    public class WordsService : ServiceBase<Word>, IWordsService
    {
        private readonly IRepository<WordBook> _wordBookRepository;
        private readonly IRepository<Line> _lineRepository;
        private readonly IRepository<Page> _pageRepository;

        public WordsService(IUnitOfWork uoW, IRepository<Word> repository, IMapper mapper, ILog logger, IRepository<WordBook> wordBookRepository, IRepository<Line> lineRepository, IRepository<Page> pageRepository)
            : base(uoW, repository, mapper, logger)
        {
            _wordBookRepository = wordBookRepository;
            _lineRepository = lineRepository;
            _pageRepository = pageRepository;
        }

        public async Task<IEnumerable<TDto>> GetAll<TDto>()
        {
            return await Get<TDto>(null, sortBy: "Name");
        }

        public async Task<PagedListDto<TDto>> GetAllPaged<TDto>(GetSortedFilteredPaging param)
        {
            return await GetPaged<TDto>(param, null);
        }

        public async Task<TDto> GetById<TDto>(int id)
        {
            return await base.GetById<TDto>(id);
        }

        public async Task Delete(int id)
        {
            await base.Delete(id);
        }

        public async Task Delete(IEnumerable<int> ids)
        {
            await base.Delete(ids);
        }

        public async Task<int> Create(WordRequest request)
        {
            var word = new Word
            {
                DateCreate = DateTime.UtcNow,
                Name = request.Name,
                WordBooks = request.WordBooks?.Select(w => new WordBook
                {
                    BookId = w.BookId,
                    Book = w.BookId == 0 ? new Book
                    {
                        Name = w.Book?.Name,
                        DateCreate = DateTime.UtcNow
                    } : null,
                    Pages = w.Pages?.Select(p => new Page
                    {
                        Number = p.Number,
                        RowId = p.RowId,
                        DateRecord = p.DateRecord,
                        Lines = p.Lines?.Select(l => new Line
                        {
                            Number = l.Number,
                            Up = l.Up
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return await Create(word);
        }

        public async Task Update(WordRequest request)
        {
            try
            {
                var word = await Repository.Where(l => l.Id == request.Id).SingleOrDefaultAsync();
                if (word.Name != request.Name) { word.Name = request.Name; }

                UpdateWordBooks(word, request.WordBooks);
                await Uow.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger.Error($"Ошибка обновления данных слова. Id: {request.Id}, entity: {JsonConvert.SerializeObject(request)}", ex);
                throw;
            }
        }


        #region [Private]
        private void UpdateWordBooks(Word word, IEnumerable<WordBookDto> wordBooks)
        {
            var wordBookDtos = wordBooks as IList<WordBookDto> ?? wordBooks.ToList();
            //add/edit

            foreach (var wordBookDto in wordBookDtos)
            {
                var wb = word.WordBooks?.SingleOrDefault(w => w.Id == wordBookDto.Id || w.BookId == wordBookDto.BookId);

                if (wb != null)
                {
                    if (!string.IsNullOrEmpty(wordBookDto.Book?.Name))
                    {
                        wb.Book = new Book
                        {
                            Name = wordBookDto.Book?.Name,
                            DateCreate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        wb.BookId = wordBookDto.BookId;
                    }

                    var pages = wordBookDto.Pages?.ToList() ?? new List<PageDto>();
                    foreach (var page in pages)
                    {
                        if (page.Id == 0)
                        {
                            wb.Pages.Add(new Page
                            {
                                Number = page.Number,
                                RowId = page.RowId == 0 ? null : page.RowId,
                                DateRecord = page.DateRecord,
                                Lines = page.Lines?.Select(l => new Line
                                {
                                    Id = l.Id,
                                    Number = l.Number,
                                    Up = l.Up
                                }).ToList()
                            });
                        }
                        else
                        {
                            var pageEnt = wb.Pages.FirstOrDefault(p => p.Id == page.Id);
                            if (pageEnt == null) throw new EntityNotFoundException();

                            pageEnt.Lines = page.Lines?.Where(l => l.Id == 0)?.Select(l => new Line
                            {
                                Id = l.Id,
                                Number = l.Number,
                                Up = l.Up
                            }).ToList();
                        }
                    }
                }
                else
                {
                    var newWb = new WordBook
                    {
                        Pages = wordBookDto.Pages?.Select(p => new Page
                        {
                            Number = p.Number,
                            RowId = p.RowId == 0 ? null : p.RowId,
                            DateRecord = p.DateRecord,
                            Lines = p.Lines?.Select(l => new Line
                            {
                                Number = l.Number,
                                Up = l.Up
                            }).ToList()
                        }).ToList()
                    };
                    if (!string.IsNullOrEmpty(wordBookDto.Book?.Name))
                    {
                        newWb.Book = new Book
                        {
                            Name = wordBookDto.Book?.Name,
                            DateCreate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        newWb.BookId = wordBookDto.BookId;
                    }
                    word.WordBooks.Add(newWb);
                    continue;
                }

                //delete
                if (wb != null)
                {

                    continue;
                }
            }

            //delete
            foreach (var wordBook in word.WordBooks.Where(wb => wordBookDtos.Any(dto => dto.BookId == wb.BookId)).ToList())
            {
                foreach (var page in wordBook.Pages.Where(pg => wordBookDtos.Any(dto => dto.BookId == wordBook.BookId && dto.Pages.Any(p => (p.Id == pg.Id)))).ToList())
                {
                    var lines = page.Lines?.Where(l => wordBookDtos.Any(wb => wb.Pages.Any(p => p.Id == page.Id && (!(p.Lines?.Any(ldto => ldto.Id == l.Id || (ldto.Number == l.Number && ldto.Up == l.Up)) ?? true) && p.Lines.Any())))).ToList();
                    if (page.Lines?.Count() > 1 && lines.Any() && lines.Count < page.Lines?.Count())
                    {
                        foreach (var line in lines)
                        {
                            _lineRepository.Remove(line);
                        }
                    }
                }
                foreach (var page in wordBook.Pages.Where(pg => wordBookDtos.Any(dto => dto.BookId == wordBook.BookId && dto.Pages.All(p => (p.Id != pg.Id)))).ToList())
                {
                    _pageRepository.Remove(page);
                }
            }
            foreach (var wordBook in word.WordBooks.Where(wb => wordBookDtos.All(dto => dto.BookId != wb.BookId)).ToList())
            {
                _wordBookRepository.Remove(wordBook);
            }
        }

        #endregion
    }
}
