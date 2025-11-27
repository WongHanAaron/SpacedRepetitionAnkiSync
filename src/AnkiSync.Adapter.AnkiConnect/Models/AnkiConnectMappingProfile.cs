using AnkiSync.Application.Ports.Anki;
using AnkiSync.Domain;
using AutoMapper;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// AutoMapper profile for mapping between application requests and AnkiConnect DTOs
/// </summary>
public class AnkiConnectMappingProfile : Profile
{
    public AnkiConnectMappingProfile()
    {
        // TestConnectionRequest -> TestConnectionRequestDto
        CreateMap<TestConnectionRequest, TestConnectionRequestDto>();

        // GetDecksRequest -> GetDecksRequestDto
        CreateMap<GetDecksRequest, GetDecksRequestDto>();

        // CreateDeckRequest -> CreateDeckRequestDto
        CreateMap<CreateDeckRequest, CreateDeckRequestDto>()
            .ConstructUsing((src, ctx) => new CreateDeckRequestDto(src.DeckName));

        // AddNoteRequest -> AddNoteRequestDto
        CreateMap<AddNoteRequest, AddNoteRequestDto>()
            .ConstructUsing((src, ctx) => new AddNoteRequestDto(ctx.Mapper.Map<AnkiNoteDto>(src.Note)));

        // AnkiNote -> AnkiNoteDto
        CreateMap<AnkiNote, AnkiNoteDto>();

        // UpdateNoteRequest -> UpdateNoteRequestDto
        CreateMap<UpdateNoteRequest, UpdateNoteRequestDto>()
            .ConstructUsing((src, ctx) => new UpdateNoteRequestDto(src.NoteId, src.Fields));

        // FindNotesRequest -> FindNotesRequestDto
        CreateMap<FindNotesRequest, FindNotesRequestDto>()
            .ConstructUsing((src, ctx) => new FindNotesRequestDto(src.Query));
    }
}