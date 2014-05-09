using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//      this is where the profiles are saved on my comp in a txt file
//      C:\Users\Garrett Kliewer\AppData\Roaming\Microsoft\PhotoBombXL\1.0.0.0

// f8aba955462676b149de52eecf12ba18d49825b2
namespace PhotoBombXL
{
    public partial class Form1 : Form
    {
        // these handle the file browsing
        private FolderBrowserDialog folderBrowserDialogInputDestination;
        private FolderBrowserDialog folderBrowserDialogOutputDestination;
        private OpenFileDialog openFileDialogImport;

        private bool isCreating = true;

        public Form1()
        {

            InitializeComponent();

            // load the profiles into the list box from the file
            loadProfilesFromFile();

            // set the combo box options
            cmbFileType.DataSource = Enum.GetValues(typeof(Profile.fileTypes));
            cmbExifMaintained.DataSource = Enum.GetValues(typeof(Profile.exifMaintained));
            cmbFileSize.DataSource = Enum.GetValues(typeof(Profile.fileSizeIndicator));

            // attempt to set default selection
            try
            {
                lstProfile.SetSelected(0, true);
            }
            catch (Exception)
            {

            }

            // this makes the profile list box use the name of the profile as its text
            lstProfile.DisplayMember = "name";
            chklstFiles.DisplayMember = "fileName";

            chkDefaultSave.Checked = true;

            // init the folder browser dialog
            folderBrowserDialogInputDestination = new FolderBrowserDialog();
            openFileDialogImport = new OpenFileDialog();
            folderBrowserDialogInputDestination.Description = "Select where your images to be converted are";
            folderBrowserDialogInputDestination.ShowNewFolderButton = false;
            openFileDialogImport.Title = "Open Text File";
            openFileDialogImport.Filter = "TXT files|*.txt";
            openFileDialogImport.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            folderBrowserDialogInputDestination.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            txtSelectDirectory.Text = folderBrowserDialogInputDestination.SelectedPath;
            populateListboxWithImageFiles();
            DisableProfile();

            folderBrowserDialogOutputDestination = new FolderBrowserDialog();
            folderBrowserDialogOutputDestination.Description = "Select where to save your converted images";
            folderBrowserDialogOutputDestination.ShowNewFolderButton = true;
            //folderBrowserDialogOutputDestination.SelectedPath = use this to set the default folder, i don't know what to put here
            //txtSaveDirectory.Text = folderBrowserDialogOutputDestination.SelectedPath;
        }

        // this populates the list box with profiles from the txt file
        private void loadProfilesFromFile()
        {
            if(!File.Exists(Application.UserAppDataPath + "\\ProfileInfo.txt"))
            {
                return;
            }
            string data;
            try
            {
                data = System.IO.File.ReadAllText(Application.UserAppDataPath + "\\ProfileInfo.txt");
            }
            catch (IOException e)
            {
                MessageBox.Show("An error occurred loading the profiles: " + e.ToString());
                return;
            }
            String[] profileData =  data.Split('\v');
            for (int i = 0; i < profileData.Length - 1; i+=9)
            {
                string name = profileData[i + 0];
                int heightInPixels = Convert.ToInt32(profileData[i + 1]);
                int widthInPixels = Convert.ToInt32(profileData[i + 2]);
                Profile.fileTypes fileType = (Profile.fileTypes)Enum.Parse(typeof(Profile.fileTypes), profileData[i + 3]);
                double fileSize = Convert.ToDouble(profileData[i + 4]);
                Profile.fileSizeIndicator indicator = (Profile.fileSizeIndicator)Enum.Parse(typeof(Profile.fileSizeIndicator), profileData[i + 5]);
                int aspectHeight = Convert.ToInt32(profileData[i + 6]);
                int aspectWidth = Convert.ToInt32(profileData[i + 7]);
                bool maintainExif = bool.Parse(profileData[i + 8]);

                Profile p = new Profile(name, heightInPixels, widthInPixels, fileType, fileSize, indicator, aspectHeight, aspectWidth, maintainExif);
                lstProfile.Items.Add(p);
            }
        }

        private void saveProfilesToFile()
        {
            // make sure we have at least one profile to save
            int numOfProfiles = lstProfile.Items.Count;

            // this string will be written to a txt file
            string profileData = "";

            // getting profiles from list
            for (int i = 0; i < numOfProfiles; i++)
            {
                profileData += ((Profile)lstProfile.Items[i]).name + "\v";
                profileData += ((Profile)lstProfile.Items[i]).heightInPixels + "\v";
                profileData += ((Profile)lstProfile.Items[i]).widthInPixels + "\v";
                profileData += ((Profile)lstProfile.Items[i]).fileType + "\v";
                profileData += ((Profile)lstProfile.Items[i]).fileSize + "\v";
                profileData += ((Profile)lstProfile.Items[i]).indicator + "\v";
                profileData += ((Profile)lstProfile.Items[i]).aspectHeight + "\v";
                profileData += ((Profile)lstProfile.Items[i]).aspectWidth + "\v";
                profileData += ((Profile)lstProfile.Items[i]).isExifMaintained + "\v";
            }

            try
            {
                // WriteAllLines creates a file, writes a collection of strings to the file,
                // and then closes the file.
                System.IO.File.WriteAllText(Application.UserAppDataPath + "\\ProfileInfo.txt", profileData);
            }
            catch (IOException e)
            {
                MessageBox.Show("An error occurred saving the profiles: " + e.ToString());
            }
        }

        private void populateListboxWithImageFiles()
        {
            // clear the list box
            chklstFiles.Items.Clear();
            string[] files = new string[0];

            // get the files
            try
            {
                files = Directory.GetFiles(txtSaveDirectory.Text);
            }
            catch (Exception)
            {
            }

            // populate the list box with files
            foreach (string file in files)
            {
                // display only image files
                if (string.Equals(Path.GetExtension(file), ".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".raw", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".gif", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".png", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".bmp", StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(Path.GetExtension(file), ".tiff", StringComparison.CurrentCultureIgnoreCase))

                {
                    ImageFilePathUtil path = new ImageFilePathUtil(file);
                    chklstFiles.Items.Add(path);
                }
            }
        }

        /***************************************************
         *      Handle closing of the app                  *
         * ************************************************/
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            saveProfilesToFile();
        }

        //   *************
        //   * GUI LOGIC *
        //   *************

        private void btnCreateProfile_Click(object sender, EventArgs e)
        {
            btnDeleteProfile.Enabled = false;
            lstProfile.Enabled = false;
            btnConvert.Enabled = false;
            isCreating = true;
            ClearProfile();
            EnableProfile();
            btnCreateProfile.Visible = false;
            btnEditProfile.Visible = false;
            btnSaveProfile.Visible = true;
            btnCancelProfile.Visible = true;
        }

        private void ClearProfile()
        {
            txtProfileName.Text = "";
            txtHeight.Text = "";
            txtFileSize.Text = "";
            cmbExifMaintained.Text = "";
            cmbFileSize.Text = "";
            cmbFileType.Text = "mb";
        }

        private void EnableProfile()
        {
            if ((Profile.fileTypes)cmbFileType.SelectedValue == Profile.fileTypes.JPG)
            {
                txtFileSize.ReadOnly = false;
                cmbFileSize.Enabled = true;
            }
            txtProfileName.ReadOnly = false;
            txtHeight.ReadOnly = false;
            cmbExifMaintained.Enabled = true;
            cmbFileType.Enabled = true;
        }

        private void DisableProfile()
        {
            txtProfileName.ReadOnly = true;
            txtHeight.ReadOnly = true;
            txtFileSize.ReadOnly = true;
            cmbExifMaintained.Enabled = false;
            cmbFileSize.Enabled = false;
            cmbFileType.Enabled = false;
        }

        private void chkDefaultSave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDefaultSave.Checked == true)
            {
                btnBrowseSave.Enabled = false;
                txtSaveDirectory.Enabled = false;
                txtSaveDirectory.Text = txtSelectDirectory.Text;
            }
            else
            {
                btnBrowseSave.Enabled = true;
                txtSaveDirectory.Enabled = true;
                txtSaveDirectory.Text = "";
            }
        }

        private void lstProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProfile.SelectedItem == null)
            {
                return;
            }

            txtProfileName.Text = ((Profile)lstProfile.SelectedItem).name;
            txtHeight.Text = ((Profile)lstProfile.SelectedItem).heightInPixels.ToString();
            cmbFileType.Text = ((Profile)lstProfile.SelectedItem).fileType.ToString();
            txtFileSize.Text = ((Profile)lstProfile.SelectedItem).fileSize.ToString();
            cmbFileSize.Text = ((Profile)lstProfile.SelectedItem).indicator.ToString();
            cmbExifMaintained.Text = ((Profile)lstProfile.SelectedItem).isExifMaintained == true ? "Yes" : "No";

            if (((Profile)lstProfile.SelectedItem).heightInPixels.ToString() == "-1")
                txtHeight.Text = "";
            if (((Profile)lstProfile.SelectedItem).fileSize.ToString() == "-1")
                txtFileSize.Text = "";
            DisableProfile();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you'd like to delete the select profile?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;
            lstProfile.Items.Remove((Profile)lstProfile.SelectedItem);

            if (lstProfile.Items.Count == 0)
            {
                ClearProfile();
                return;
            }

            SortList();
            SelectTop();
            saveProfilesToFile();
        }

        private void btnBrowseSelect_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogInputDestination.ShowDialog() == DialogResult.OK)
            {
                // sets the text box with the path
                txtSelectDirectory.Text = folderBrowserDialogInputDestination.SelectedPath;

                populateListboxWithImageFiles();
            }
        }

        private void btnBrowseSave_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogOutputDestination.ShowDialog() == DialogResult.OK)
            {
                txtSaveDirectory.Text = folderBrowserDialogOutputDestination.SelectedPath;
            }
        }

        private void chklstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pctrPreviewImage.Image = Image.FromFile(((ImageFilePathUtil)chklstFiles.SelectedItem).fullPath);
            }
            catch (Exception exception)
            {
                // sometimes the path gets messed up but it's not really necessary to handle
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chklstFiles.Items.Count; i++)
                chklstFiles.SetItemChecked(i, true);
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chklstFiles.Items.Count; i++)
                chklstFiles.SetItemChecked(i, false);
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            Profile.fileTypes fileType = (Profile.fileTypes)Enum.Parse(typeof(Profile.fileTypes), cmbFileType.Text);
            Profile.exifMaintained exifMaintained = (Profile.exifMaintained)Enum.Parse(typeof(Profile.exifMaintained), cmbExifMaintained.Text);
            Profile.fileSizeIndicator indicator = (Profile.fileSizeIndicator)Enum.Parse(typeof(Profile.fileSizeIndicator), cmbFileSize.Text);

            bool isExifMaintained = exifMaintained == Profile.exifMaintained.Yes ? true : false;

            Profile p;

            if (txtProfileName.Text == "")
            {
                MessageBox.Show("Profile does not currently have a name.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                if (Convert.ToInt32(txtHeight.Text) < 1)
                {
                    MessageBox.Show("Values smaller than one cannot be used.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }
            catch (Exception)
            {
                
            }

            try
            {
            if (Convert.ToDouble(txtFileSize.Text) <= 0)
                {
                    MessageBox.Show("Values smaller than one cannot be used.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            catch (Exception)
            {
                
            }

            if (txtHeight.Text == "") txtHeight.Text = "-1";
            if (txtFileSize.Text == "") txtFileSize.Text = "-1";

            try
            {
                p = new Profile(txtProfileName.Text, Convert.ToInt32(txtHeight.Text), 1, fileType, Convert.ToDouble(txtFileSize.Text), indicator, 0, 0, isExifMaintained);
            }
            catch (Exception)
            {
                txtHeight.Text = "";
                txtFileSize.Text = "";
                MessageBox.Show("A field is currently invalid. The profile has not been created.", "Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (isCreating == false) lstProfile.Items.Remove(lstProfile.SelectedItem);
            lstProfile.Items.Add(p);
            btnCancelProfile_Click(sender, e);
            DisableProfile();
            btnConvert.Enabled = true;
            btnDeleteProfile.Enabled = true;
            lstProfile.Enabled = true;
            saveProfilesToFile();
        }

        private void btnCancelProfile_Click(object sender, EventArgs e)
        {
            btnConvert.Enabled = true;
            btnDeleteProfile.Enabled = true;
            lstProfile.Enabled = true;
            DisableProfile();
            btnCreateProfile.Visible = true;
            btnEditProfile.Visible = true;
            btnCancelProfile.Visible = false;
            btnSaveProfile.Visible = false;

            SortList();
            SelectTop();
        }

        private void SortList()
        {
            List<Profile> list = lstProfile.Items.Cast<Profile>().ToList();
            List<Profile> sortedList = list.OrderBy(o => o.name).ToList();
            lstProfile.Items.Clear();
            lstProfile.Items.AddRange(sortedList.ToArray());
        }

        private void SelectTop()
        {
            try
            {
                lstProfile.SetSelected(0, true);
            }
            catch (Exception)
            {

            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            btnConvert.Enabled = false;
            lstProfile.Enabled = false;
            btnDeleteProfile.Enabled = false;
            isCreating = false;
            EnableProfile();
            btnCreateProfile.Visible = false;
            btnEditProfile.Visible = false;
            btnSaveProfile.Visible = true;
            btnCancelProfile.Visible = true;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (chklstFiles.Items.Count == 0)
            {
                MessageBox.Show("No images to convert", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (chklstFiles.CheckedItems.Count == 0)
            {
                MessageBox.Show("No images selected to be converted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtSaveDirectory.Text == "")
            {
                MessageBox.Show("Since no directory was selected, a default directory will be used.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDefaultSave.Checked = true;
            }

            ConverterUtil.convertFiles(chklstFiles.CheckedItems.Cast<ImageFilePathUtil>().ToList(), (Profile)lstProfile.SelectedItem, txtSaveDirectory.Text, prgProgressBar);
            MessageBox.Show("Conversion Complete");
            prgProgressBar.Value = 0;
        }

        private void txtSelectDirectory_TextChanged(object sender, EventArgs e)
        {
            chkDefaultSave_CheckedChanged(sender, e);

            populateListboxWithImageFiles();
        }

        private void cmbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFileSize.Text = "";
            if ((Profile.fileTypes)cmbFileType.SelectedValue == Profile.fileTypes.JPG)
            {
                txtFileSize.ReadOnly = false;
                cmbFileSize.Enabled = true;
            }
            else
            {
                cmbFileSize.Enabled = false;
                txtFileSize.ReadOnly = true;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Importing profiles will overwrite your current profiles, are you sure you would like to do this?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                if (openFileDialogImport.ShowDialog() == DialogResult.OK)
                {
                    string profiles = openFileDialogImport.FileName;

                    string line = "";

                    try
                    {
                        using (StreamReader sr = new StreamReader(profiles))
                        {
                            line = sr.ReadToEnd();
                            Console.WriteLine(line);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error importing file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    System.IO.File.WriteAllText(Application.UserAppDataPath + "\\ProfileInfo.txt", line);
                    lstProfile.Items.Clear();
                    loadProfilesFromFile();
                }
            }
            else
            {
                return;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you would like to export your profile list?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                System.IO.File.Copy(Application.UserAppDataPath + "\\ProfileInfo.txt", System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + "\\ProfileInfo.txt");
            }
        }

        private void btn_help_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.html");
        }
    }
}
