using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DaisyMvc.Models;
using DaisyMvc.Models.ViewModels;
using Newtonsoft.Json;

namespace DaisyMvc.Controllers
{


    public class VideoController : Controller
    {

        private readonly string DIRPATH = System.Web.HttpContext.Current.Server.MapPath("~/Content/UploadedFiles/");


        // GET: Video
        //[Route("videos")]       todo - workout why this isnt working - i should be able to change the method name and still route to this from "video/videos"
        //[Authorize]
        //public ActionResult VideosViaApi()
        //{
        //    var results = HttpClientGetData();

        //    //todo move elsewhere and make VideosVm.Videos a collection of Video objects , not string as it currently is
        //    var vvm = new VideosVm { Videos = results };


        //    //return Content("You reached the Video/Videos Action Controller");
        //    return View(vvm);
        //}


        //GET
        [Authorize]
        public ActionResult Videos()
        {

            var results = Directory.GetFiles(DIRPATH);

            var vvm = new VideosVm();
            vvm.Videos = new List<VideoFile>();

            int ctr = 0;
            foreach (var result in results)
            {
                VideoFile v = new VideoFile() { Id = ++ctr, Title = Path.GetFileName(result) };
                vvm.Videos.Add(v);
            }

            //return Content("You reached the Video/Videos Action Controller");
            return View(vvm);
        }



        //POST

        private string uploadedFilesPath = "~/Content/UploadedFiles";

        [HttpPost]
        public string MultiUpload(string id, string fileName)
        {
            var chunkNumber = id;
            var chunks = Request.InputStream;
            string path = Server.MapPath(uploadedFilesPath + "/Temp");
            string newpath = Path.Combine(path, fileName + chunkNumber);
            using (FileStream fs = System.IO.File.Create(newpath))
            {
                byte[] bytes = new byte[AppValues.FileChunkSz];
                int bytesRead;
                while ((bytesRead = Request.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
            }
            return "done";
        }

        [HttpPost]
        public string UploadComplete(string fileName, string complete)
        {
            string tempPath = Server.MapPath(uploadedFilesPath + "/Temp");
            string videoPath = Server.MapPath(uploadedFilesPath);
            string newPath = Path.Combine(tempPath, fileName);
            if (complete == "1")
            {
                string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
                foreach (string filePath in filePaths)
                {
                    MergeFiles(newPath, filePath);
                }
            }
            System.IO.File.Move(Path.Combine(tempPath, fileName), Path.Combine(videoPath, fileName));
            return "success";
        }

        private static void MergeFiles(string file1, string file2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(file1, FileMode.Append);
                fs2 = System.IO.File.Open(file2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(file2);
            }
        }


        [HttpPost]
        public ActionResult UploadFiles2(HttpPostedFileBase file)
        {

            //stick it in a collection to pass to SaveFiles
            SaveFiles(new List<HttpPostedFileBase>() { file });

            //todo not sure where to go from here at the moment....
            return Content("file uploaded successfully");
        }




        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            SaveFiles(files);
            //todo not sure where to go from here at the moment....
            return Content("file uploaded successfully");
        }

        private void SaveFiles(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                // in case it has the path....
                string fname = Path.GetFileName(file.FileName);

                string fullSavePath = DIRPATH + fname;
                file.SaveAs(fullSavePath);
            }
        }


        ////[Route("videos")]       todo - workout why this isnt working - i should be able to change the method name and still route to this from "video/videos"
        //public ActionResult Videos(int id)
        //{

        //    var theFile = HttpClientGetFileExample(id);

        //    Response.Clear();
        //    Response.AddHeader("Content-Length", theFile.FileStream.Length.ToString());
        //    Response.AddHeader("Content-Disposition", "attachment; filename=FILENAME");
        //    Response.BinaryWrite(theFile.FileStream); 
        //    Response.Flush();
        //    Response.End();






        //    //todo move elsewhere and make VideosVm.Videos a collection of Video objects , not string as it currently is
        //    var vvm = new VideosVm { Videos = results };


        //    //return Content("You reached the Video/Videos Action Controller");
        //    return View(vvm);
        //}



        // GET: Video/Create
        public
        ActionResult Create()
        {
            //return View("PostFile");
            return View("DragDropFilePost");
        }

        // POST: Video/Create
        //[HttpPost]
        //public ActionResult Create(PostFileVm postedFile)
        //{
        //    //return Content("Posted.");
        //    //return View("PostFile");
        //    return View("DragDropFilePost");
        //}

        // POST: Video/Create
        [HttpPost]
        public ActionResult UploadFilesViaApi(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {

                //bool ok = HttpPostToWebApi(file);
                //HttpPostToWebApi2(file);
                TryThisOne(file);


                //var filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);

                ////todo - save em
                //file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));
                ////Here you can write code for save this information in your database if you want
            }


            //todo not sure where to go from here at the moment....
            return Content("file uploaded successfully");
        }

        public ActionResult Open()
        {
            //check if we have the file locally




            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        //public class Video
        //{
        //    //[JsonProperty("Films")]
        //    public AFilm[] Films { get; set; }
        //}

        //public class AFilm
        //{
        //    [JsonProperty("id")]
        //    public int theId { get; set; }

        //    [JsonProperty("title")]
        //    public string theTitle { get; set; }

        //}

        //private async Task<HttpResponseMessage> postit(HttpPostedFileBase model)
        //{
        //    var r = new TResponse
        //    {

        //    }
        //    using (var apiResponse = await HttpClientPostToWebApi(model))
        //    {
        //        var response = await CreateJsonResponse<TResponse>(apiResponse);
        //        response.Data = Json.Decode<int>(response.ResponseResult);
        //        return response;
        //    }

        //}

        //private async Task<HttpResponseMessage> HttpClientPostToWebApi(HttpPostedFileBase model)
        //{

        //    HttpClient httpClient = new HttpClient();
        //    string BaseUri = "http://localhost:63314/";




        //    httpClient.BaseAddress = new Uri(BaseUri);
        //    httpClient.DefaultRequestHeaders.Accept.Clear();
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //    //would like to get MultiPartFormContent working....
        //    //var content = new MultipartFormDataContent();
        //    //content.Add(new StreamContent(fs), "\"video\"", "\"f2.txt\"");
        //    //var response1 = await httpClient.PostAsJsonAsync("api/MediaFiles", content);


        //    //the request uri used below would be better passed into this method..
        //    var response1 = await httpClient.PostAsJsonAsync("api/MediaFiles", model);

        //    //if (response1.Content != null)
        //    //{
        //    //    await response1.Content.ReadAsAsync<>();
        //    //}
        //    return response1;

        //}

        private bool HttpPostToWebApi(HttpPostedFileBase vf)
        {


            Stream fStream = (Stream)vf.InputStream;

            var name = "\"model\"";
            var title = "\"" + vf.FileName + "\"";


            var client = new HttpClient();

            var content = new MultipartFormDataContent();

            //content.Add(new StreamContent(stream), "\"text\"", "\"WORKED.txt\"");

            var toPost = new
            {
                Id = 777,
                Name = "Trying a full file...",
                //FileSream = (Stream) vf.InputStream
            };



            content.Add(new StreamContent(fStream), name, title);
            //content.Add(new StreamContent(stream), "\"video\"", "\"summer-evening2.jpg\"");


            var task = client.PostAsync(AppValues.ApiUri + "api/MediaFiles", content).
                ContinueWith((taskWithResponse) =>
                {
                    //this will raise an exception if no success status on the response
                    //taskWithResponse.Result.EnsureSuccessStatusCode();
                }).ContinueWith((err) =>
                {
                    //note: the ContinueWith immediately above is to handle the exception f there is one
                    if (err.Exception != null)
                    {
                        Console.WriteLine("Exception {0}", err.Exception.ToString());
                    }
                });

            task.Wait();
            return true;
        }
        //private async void HttpPostToWebApi2(HttpPostedFileBase vf)
        private async void HttpPostToWebApi3(HttpPostedFileBase vf)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:63314/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string filepath = "C:/Users/Public/Videos/Sample Videos/Wildlife.wmv";
                string filename = "Wildlife.wmv";

                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filepath));
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filename };
                content.Add(fileContent);

                HttpResponseMessage response = await client.PostAsync("api/MediaFiles", content);
                string returnString = await response.Content.ReadAsAsync<string>();
            }


        }

        private void TryThisOne(HttpPostedFileBase vf)
        {

            using (HttpClient client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    byte[] fileBytes = new byte[vf.InputStream.Length + 1]; vf.InputStream.Read(fileBytes, 0, fileBytes.Length);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = vf.FileName };
                    content.Add(fileContent);
                    var result = client.PostAsync(AppValues.ApiUri + "api/MediaFiles", content).Result;
                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        ViewBag.Message = "Created";
                    }
                    else
                    {
                        ViewBag.Message = "Failed";
                    }
                }
            }


        }
        private void HttpPostToWebApi2(HttpPostedFileBase vf)
        {


            var client = new HttpClient();

            var obj = new
            {
                Id = -999,
                Name = "FINALLY - dear god...."

            };


            var content = new StringContent(JsonConvert.SerializeObject(obj).ToString(),
                         Encoding.UTF8, "application/json");




            var task = client.PostAsync("http://localhost:63314/api/MediaFiles", content).
                ContinueWith((taskWithResponse) =>
                {
                    //this will raise an exception if no success status on the response
                    //taskWithResponse.Result.EnsureSuccessStatusCode();
                }).ContinueWith((err) =>
                {
                    //note: the ContinueWith immediately above is to handle the exception f there is one
                    if (err.Exception != null)
                    {
                        Console.WriteLine("Exception {0}", err.Exception.ToString());
                    }
                });

            task.Wait();


        }


        private VideoFile HttpClientGetFileExample(int id)
        {
            //GET DATA VIA THE WEB API

            var htpc = new HttpClient();


            VideoFile model = null; //this will become clear in the next few lines of code....


            //call a Get (async) at the Uri provided, with specified query string
            //  with httpclient, the Get returns a Task
            //var task = htpc.GetAsync("http://search.twitter.com/search.json?q=pluralsight");


            var task = htpc.GetAsync("http://localhost:63314/api/MediaFiles/" + id).ContinueWith(taskWithResult =>
              {
                  //get the result (off the returned HttpClient Task) from the Get action method we called via the Uri
                  var response = taskWithResult.Result;

                  //created my own .net type here (MyMovies) for the data to be 'bound' to , the Read (as Async) extension method on the response.content object
                  //will bind the response data to the task.Result object , type MyMovies
                  var readTask = response.Content.ReadAsAsync<VideoFile>();
                  // wait till all reading of Task is done before we move on to next line of code.
                  readTask.Wait();

                  //now readTask has Result populated
                  model = readTask.Result;
              });
            //wait until all that stuff going on with the task object is finished before we move to next line of code.
            task.Wait();

            return model;
        }
        private VideoFile[] HttpClientGetData()
        {
            //GET DATA VIA THE WEB API

            var htpc = new HttpClient();


            VideoFile[] model = null; //this will become clear in the next few lines of code....


            //call a Get (async) at the Uri provided, with specified query string
            //  with httpclient, the Get returns a Task
            //var task = htpc.GetAsync("http://search.twitter.com/search.json?q=pluralsight");


            var task = htpc.GetAsync(AppValues.ApiUri + "api/MediaFiles").ContinueWith(taskWithResult =>
              {
                  //get the result (off the returned HttpClient Task) from the Get action method we called via the Uri
                  var response = taskWithResult.Result;

                  //created my own .net type here (MyMovies) for the data to be 'bound' to , the Read (as Async) extension method on the response.content object
                  //will bind the response data to the task.Result object , type MyMovies
                  var readTask = response.Content.ReadAsAsync<VideoFile[]>();
                  // wait till all reading of Task is done before we move on to next line of code.
                  readTask.Wait();

                  //now readTask has Result populated
                  model = readTask.Result;
              });
            //wait until all that stuff going on with the task object is finished before we move to next line of code.
            task.Wait();

            return model;
        }

        //private MyMovies HttpClientGetListExampleOnOuterObject()
        //{
        //    var htpc = new HttpClient();


        //    MyMovies model = null; //this will become clear in the next few lines of code....


        //    //call a Get (async) at the Uri provided, with specified query string
        //    //  with httpclient, the Get returns a Task
        //    //var task = htpc.GetAsync("http://search.twitter.com/search.json?q=pluralsight");


        //    var task = htpc.GetAsync("http://localhost:64814/api/Movies").ContinueWith((taskWithResult) =>


        //    //now we can use the ContinueWith extension method on the task... using lamda to, in this case, act on the 'result'
        //    //task.ContinueWith((taskWithResult) =>
        //    {
        //        //get the result (off the returned HttpClient Task) from the Get action method we called via the Uri
        //        var response = taskWithResult.Result;

        //        ////created my own .net type here (MyMovies) for the data to be 'bound' to , the Read (as Async) extension method on the response.content object
        //        ////will bind the response data to the task.Result object , type MyMovies
        //        var readTask = response.Content.ReadAsAsync<MyMovies>();


        //        // wait till all reading of Task is done before we move on to next line of code.
        //        readTask.Wait();

        //        //now readTask has Result populated
        //        model = readTask.Result;

        //    });
        //    //wait until all that stuff going on with the task object is finished before we move to next line of code.
        //    task.Wait();

        //    return model;
        //}


        //private AFilm HttpCLientGetDataObjectExample()
        //{
        //    var htpc = new HttpClient();


        //    AFilm aFilm = null; //this will become clear in the next few lines of code....


        //    //get one data object
        //    var task = htpc.GetAsync("http://localhost:64814/api/Movies/1").ContinueWith((taskWithResult) =>
        //    ////now we can use the ContinueWith extension method on the task... using lamda to, in this case, act on the 'result'
        //    //task.ContinueWith((taskWithResult) =>
        //    {
        //        //get the result (off the returned HttpClient Task) from the Get action method we called via the Uri
        //        var response = taskWithResult.Result;

        //        //created my own .net type here (MyMovies) for the data to be 'bound' to , the Read (as Async) extension method on the response.content object
        //        //will bind the response data to the task.Result object , type MyMovies
        //        var readTask = response.Content.ReadAsAsync<AFilm>();
        //        // wait till all reading of Task is done before we move on to next line of code.
        //        readTask.Wait();

        //        //now readTask has Result populated
        //        aFilm = readTask.Result;
        //    });
        //    //wait until all that stuff going on with the task object is finished before we move to next line of code.
        //    task.Wait();

        //    return aFilm;
        //}
    }

    ////// GET: Video/Details/5
    ////public ActionResult Details(int id)
    ////{
    ////    return View();
    ////}

    ////// GET: Video/Create
    ////public ActionResult Create()
    ////{
    ////    return View();
    ////}

    ////// POST: Video/Create
    ////[HttpPost]
    ////public ActionResult Create(FormCollection collection)
    ////{
    ////    try
    ////    {
    ////        // TODO: Add insert logic here

    ////        return RedirectToAction("Index");
    ////    }
    ////    catch
    ////    {
    ////        return View();
    ////    }
    ////}

    ////// GET: Video/Edit/5
    ////public ActionResult Edit(int id)
    ////{
    ////    return View();
    ////}

    ////// POST: Video/Edit/5
    ////[HttpPost]
    ////public ActionResult Edit(int id, FormCollection collection)
    ////{
    ////    try
    ////    {
    ////        // TODO: Add update logic here

    ////        return RedirectToAction("Index");
    ////    }
    ////    catch
    ////    {
    ////        return View();
    ////    }
    ////}

    ////// GET: Video/Delete/5
    ////public ActionResult Delete(int id)
    ////{
    ////    return View();
    ////}

    ////// POST: Video/Delete/5
    ////[HttpPost]
    ////public ActionResult Delete(int id, FormCollection collection)
    ////{
    ////    try
    ////    {
    ////        // TODO: Add delete logic here

    ////        return RedirectToAction("Index");
    ////    }
    ////    catch
    ////    {
    ////        return View();
    ////    }
    ////}


    //#region "privates"


    ////private AFilm[] HttpClientGetListExample()
    //private Video[] HttpClientGetVideos()
    //    {
    //        var htpc = new HttpClient();

    //        //AFilm[] model = null; //this will become clear in the next few lines of code....
    //        Video[] model = null; //this will become clear in the next few lines of code....


    //        //call a Get (async) at the Uri provided, with specified query string
    //        //  with httpclient, the Get returns a Task
    //        //var task = htpc.GetAsync("http://search.twitter.com/search.json?q=pluralsight");


    //        var task = htpc.GetAsync("http://localhost:9999/api/DaisyVideos").ContinueWith((taskWithResult) =>
    //        {
    //            //get the result (off the returned HttpClient Task) from the Get action method we called via the Uri
    //            var response = taskWithResult.Result;

    //            //created my own .net type here (MyMovies) for the data to be 'bound' to , the Read (as Async) extension method on the response.content object
    //            //will bind the response data to the task.Result object , type MyMovies
    //            var readTask = response.Content.read   //ReadAsAsync<Video>();


    //            // wait till all reading of Task is done before we move on to next line of code.
    //            //readTask.Wait();

    //            //now readTask has Result populated
    //            //model = readTask.Result;
    //        });
    //        //wait until all that stuff going on with the task object is finished before we move to next line of code.
    //        task.Wait();

    //        return model;
    //    }

    //    #endregion


    //}

    //public class Video
    //{
    //    public string title { get; set; }

    //}
}
