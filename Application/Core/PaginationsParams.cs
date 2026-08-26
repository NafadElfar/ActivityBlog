using System;

namespace Application.Core;

public class PaginationsParams<TCursor>
{ 
  private const int MaxPageSiza = 50;

    public TCursor? Cursor { get; set; }
    private int _PageSiza = 3;
    public int PageSize
    {
      get => _PageSiza;
      set => _PageSiza = value > MaxPageSiza ? MaxPageSiza : value;
    }
}
